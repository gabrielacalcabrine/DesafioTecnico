using System.Text.Json.Serialization;
// TODO: Adicionar autenticação/autorização se o desafio receber usuários no futuro.
using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services;
using Trading.Application.Services.Interfaces;
using Trading.Infrastructure.Repositories;
using Trading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Trading.Api.DTOs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TradingDb")
    ?? "Host=localhost;Port=5432;Database=trading;Username=trading;Password=trading";
builder.Services.AddDbContext<TradingDbContext>(options => options.UseNpgsql(connectionString));
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
// TODO: Adicionar logging estruturado e correlation ID.
// A connection string do PostgreSQL chega via ConnectionStrings__TradingDb (ver docker-compose.yml).

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trading API v1");
    options.RoutePrefix = "swagger";
});
app.MapControllers();

// Obrigatório para a suíte de avaliação: 200 quando a API está pronta.
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

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
