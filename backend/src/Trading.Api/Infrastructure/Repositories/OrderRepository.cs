using Microsoft.EntityFrameworkCore;
using Trading.Application.Repositories.Interfaces;
using Trading.Domain.Entities;
using Trading.Domain.Enums;
using Trading.Infrastructure.Persistence;

namespace Trading.Infrastructure.Repositories;

public sealed class OrderRepository(TradingDbContext db) : IOrderRepository
{
    // TODO: Substituir EnsureCreated por migrations versionadas antes da produção.
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (db.Entry(order).State == EntityState.Detached) db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Order>> ListAsync(string? asset = null, string? status = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = db.Orders;
        if (!string.IsNullOrWhiteSpace(asset)) query = query.Where(x => x.Asset == asset.Trim().ToUpper());
        if (Enum.TryParse<OrderStatus>(status, true, out var parsed)) query = query.Where(x => x.Status == parsed);
        return await query.OrderBy(x => x.CreatedAt).ToArrayAsync(cancellationToken);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        db.Orders.RemoveRange(db.Orders);
        await db.SaveChangesAsync(cancellationToken);
    }
}
