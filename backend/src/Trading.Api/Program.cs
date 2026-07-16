var builder = WebApplication.CreateBuilder(args);

// TODO: registre aqui seus serviços (DbContext, repositórios, casos de uso, validação, logging estruturado...).
// A connection string do PostgreSQL chega via ConnectionStrings__TradingDb (ver docker-compose.yml).

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();

// Obrigatório para a suíte de avaliação: 200 quando a API está pronta.
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// TODO: implemente o contrato de API descrito no README:
//   POST /orders
//   GET  /orders/{id}   |  GET /orders
//   GET  /orderbook/{ativo}
//   GET  /trades
//   POST /orders/{id}/cancel
//   POST /admin/reset   (apenas quando ENABLE_TEST_ENDPOINTS=true)

app.Run();
