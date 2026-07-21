using Microsoft.EntityFrameworkCore;
using Trading.Application.Repositories.Interfaces;
using Trading.Domain.Entities;
using Trading.Infrastructure.Persistence;

namespace Trading.Infrastructure.Repositories;

public sealed class TradeRepository(TradingDbContext db) : ITradeRepository
{
    // TODO: Participar da mesma transação do matching para garantir consistência entre ordens e trades.
    public async Task AddAsync(Trade trade, CancellationToken cancellationToken = default)
    {
        if (db.Entry(trade).State == EntityState.Detached) db.Trades.Add(trade);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Trade>> ListAsync(string? asset = null, DateTimeOffset? start = null, DateTimeOffset? end = null, Guid? orderId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Trade> query = db.Trades;
        var utcStart = start?.ToUniversalTime();
        var utcEnd = end?.ToUniversalTime();
        if (!string.IsNullOrWhiteSpace(asset)) query = query.Where(x => x.Asset == asset.Trim().ToUpper());
        if (utcStart.HasValue) query = query.Where(x => x.ExecutedAt >= utcStart.Value);
        if (utcEnd.HasValue) query = query.Where(x => x.ExecutedAt <= utcEnd.Value);
        if (orderId.HasValue) query = query.Where(x => x.BuyOrderId == orderId || x.SellOrderId == orderId);
        return await query.OrderBy(x => x.ExecutedAt).ToArrayAsync(cancellationToken);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        db.Trades.RemoveRange(db.Trades);
        await db.SaveChangesAsync(cancellationToken);
    }
}
