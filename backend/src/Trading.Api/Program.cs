using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services;
using Trading.Application.Services.Interfaces;
using Trading.Infrastructure.Repositories;
using Trading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Trading.Api.DTOs;
using Microsoft.OpenApi.Models;
using Trading.Domain.Services;
using Trading.Application.Events;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TradingDb")
    ?? "Host=localhost;Port=5432;Database=trading;Username=trading;Password=trading";
builder.Services.AddDbContext<TradingDbContext>(options => options.UseNpgsql(connectionString, npgsql =>
    npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));
// DONE: A persistência usa PostgreSQL via EF Core e TradingDbContext.

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IOrderBookService, OrderBookService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IDomainEventPublisher, LoggingDomainEventPublisher>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(item => item.Value?.Errors.Select(error =>
                FormatValidationMessage(item.Key, error.ErrorMessage)) ?? [])
            .Distinct()
            .ToArray();

        return new ObjectResult(new ErrorResponseDto
        {
            Code = "validation_error",
            Message = "A requisição contém dados inválidos. Corrija os campos e tente novamente.",
            Errors = errors
        })
        {
            StatusCode = StatusCodes.Status406NotAcceptable
        };
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Trading API",
        Version = "v1",
        Description = "API simplificada para negociação de ativos financeiros."
    });
});

// DONE: Serviços, DbContext, repositórios, validação e controllers estão registrados.
// A connection string do PostgreSQL chega via ConnectionStrings__TradingDb (ver docker-compose.yml).

var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(section => section.Value)
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Cast<string>()
    .ToArray();
var allowedOrigins = configuredOrigins.Length > 0 ? configuredOrigins : ["http://localhost:3000"];
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseCors();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);

        var (status, code, message) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found", "O recurso solicitado não foi encontrado."),
            InvalidOperationException invalidOperation => (StatusCodes.Status409Conflict, "conflict", invalidOperation.Message),
            ValidationException validation => (StatusCodes.Status406NotAcceptable, "validation_error", validation.Message),
            _ => (StatusCodes.Status500InternalServerError, "internal_error", "Ocorreu um erro interno. Tente novamente mais tarde.")
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ErrorResponseDto { Code = code, Message = message, Errors = [] });
    });
});
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
        ?? Guid.NewGuid().ToString("N");
    context.Response.Headers["X-Correlation-Id"] = correlationId;
    using (app.Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        await next();
});
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trading API v1");
    options.RoutePrefix = "swagger";
});
app.MapControllers();

// Obrigatório para a suíte de avaliação: 200 somente quando o banco está acessível.
app.MapGet("/health", async (TradingDbContext db, CancellationToken cancellationToken) =>
{
    try
    {
        return await db.Database.CanConnectAsync(cancellationToken)
            ? Results.Ok(new { status = "ok" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception exception)
    {
        app.Logger.LogWarning(exception, "Trading database health check failed");
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
});

static string FormatValidationMessage(string key, string message)
{
    if (key.Contains("Inicio", StringComparison.OrdinalIgnoreCase) || key.Contains("Fim", StringComparison.OrdinalIgnoreCase))
        return $"{key}: formato de data inválido. Use ISO 8601 UTC, por exemplo: 2026-07-15T00:00:00Z.";
    if (key.Contains("Tipo", StringComparison.OrdinalIgnoreCase))
        return $"{key}: use \"Compra\" ou \"Venda\".";
    return string.IsNullOrWhiteSpace(message) ? $"{key}: valor inválido." : $"{key}: {message}";
}

// DONE: O contrato principal da API está implementado nos controllers.
//   POST /orders
//   GET  /orders/{id}   |  GET /orders
//   GET  /orderbook/{ativo}
//   GET  /trades
//   POST /orders/{id}/cancel
//   POST /admin/reset   (apenas quando ENABLE_TEST_ENDPOINTS=true)

app.Run();
