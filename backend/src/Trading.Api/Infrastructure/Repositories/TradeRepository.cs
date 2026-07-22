using Microsoft.EntityFrameworkCore;
using Trading.Application.Repositories.Interfaces;
using Trading.Domain.Entities;
using Trading.Infrastructure.Persistence;
using Trading.Application.Models;

namespace Trading.Infrastructure.Repositories;

public sealed class TradeRepository(TradingDbContext db) : ITradeRepository
{
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

    public async Task<PagedResult<Trade>> ListPageAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<Trade> query = db.Trades.AsNoTracking();
        var utcStart = start?.ToUniversalTime();
        var utcEnd = end?.ToUniversalTime();
        if (!string.IsNullOrWhiteSpace(asset)) query = query.Where(x => x.Asset == asset.Trim().ToUpperInvariant());
        if (utcStart.HasValue) query = query.Where(x => x.ExecutedAt >= utcStart.Value);
        if (utcEnd.HasValue) query = query.Where(x => x.ExecutedAt <= utcEnd.Value);
        if (orderId.HasValue) query = query.Where(x => x.BuyOrderId == orderId || x.SellOrderId == orderId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.ExecutedAt).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(cancellationToken);
        return new PagedResult<Trade>(items, page, pageSize, total);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        db.Trades.RemoveRange(db.Trades);
        await db.SaveChangesAsync(cancellationToken);
    }
}
