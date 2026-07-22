using Microsoft.EntityFrameworkCore;
using Trading.Application.Services.Interfaces;

namespace Trading.Infrastructure.Persistence;

public sealed class EfTransactionManager(TradingDbContext db) : ITransactionManager
{
    public async Task ExecuteConsistentAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.RepeatableRead,
                cancellationToken);

            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
