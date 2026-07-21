using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Infrastructure.Persistence;
namespace Trading.Application.Services;

// DONE: O reset limpa trades antes de ordens para respeitar as chaves estrangeiras.
// TODO: Envolver a limpeza das tabelas em uma transação explícita.
public sealed class AdminService(IOrderRepository orders, ITradeRepository trades, TradingDbContext? db = null) : IAdminService
{
    public async Task ResetAsync(CancellationToken cancellationToken = default) 
    { 
        if (db is not null)
        {
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync<object, int>(new object(), async (_, _, ct) =>
            {
                await ResetCoreAsync(ct);
                return 0;
            }, null, cancellationToken);
            return;
        }
        await ResetCoreAsync(cancellationToken);
    }

    private async Task ResetCoreAsync(CancellationToken cancellationToken)
    {
        await using var transaction = db is null ? null : await db.Database.BeginTransactionAsync(cancellationToken);
        await trades.DeleteAllAsync(cancellationToken);
        await orders.DeleteAllAsync(cancellationToken);
        if (transaction is not null) await transaction.CommitAsync(cancellationToken);
    }
}
