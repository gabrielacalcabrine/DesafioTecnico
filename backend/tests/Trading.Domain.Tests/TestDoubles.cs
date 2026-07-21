using Trading.Application.Repositories.Interfaces;
using Trading.Domain.Entities;

namespace Trading.Domain.Tests;

internal sealed class FakeOrderRepository : IOrderRepository
{
    public List<Order> Items { get; } = [];
    public Task AddAsync(Order order, CancellationToken cancellationToken = default) { if (!Items.Contains(order)) Items.Add(order); return Task.CompletedTask; }
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.SingleOrDefault(x => x.Id == id));
    public Task<IReadOnlyCollection<Order>> ListAsync(string? asset = null, string? status = null, CancellationToken cancellationToken = default)
    {
        IEnumerable<Order> result = Items;
        if (!string.IsNullOrWhiteSpace(asset)) result = result.Where(x => x.Asset == asset.Trim().ToUpperInvariant());
        if (Enum.TryParse<Trading.Domain.Enums.OrderStatus>(status, true, out var parsed)) result = result.Where(x => x.Status == parsed);
        return Task.FromResult<IReadOnlyCollection<Order>>(result.ToArray());
    }
    public Task DeleteAllAsync(CancellationToken cancellationToken = default) { Items.Clear(); return Task.CompletedTask; }
}

internal sealed class FakeTradeRepository : ITradeRepository
{
    public List<Trade> Items { get; } = [];
    public Task AddAsync(Trade trade, CancellationToken cancellationToken = default) { Items.Add(trade); return Task.CompletedTask; }
    public Task<IReadOnlyCollection<Trade>> ListAsync(string? asset = null, DateTimeOffset? start = null, DateTimeOffset? end = null, Guid? orderId = null, CancellationToken cancellationToken = default)
    {
        IEnumerable<Trade> result = Items;
        if (!string.IsNullOrWhiteSpace(asset)) result = result.Where(x => x.Asset == asset.Trim().ToUpperInvariant());
        if (start.HasValue) result = result.Where(x => x.ExecutedAt >= start);
        if (end.HasValue) result = result.Where(x => x.ExecutedAt <= end);
        if (orderId.HasValue) result = result.Where(x => x.BuyOrderId == orderId || x.SellOrderId == orderId);
        return Task.FromResult<IReadOnlyCollection<Trade>>(result.ToArray());
    }
    public Task DeleteAllAsync(CancellationToken cancellationToken = default) { Items.Clear(); return Task.CompletedTask; }
}

internal sealed class FakeMatchingService : Trading.Application.Services.Interfaces.IMatchingService
{
    public List<Order> MatchedOrders { get; } = [];
    public Task MatchAsync(Order incomingOrder, CancellationToken cancellationToken = default) { MatchedOrders.Add(incomingOrder); return Task.CompletedTask; }
}
