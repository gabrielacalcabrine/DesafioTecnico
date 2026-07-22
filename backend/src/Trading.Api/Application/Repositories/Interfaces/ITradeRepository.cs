using Trading.Domain.Entities;
using Trading.Application.Models;

namespace Trading.Application.Repositories.Interfaces;

// TODO: Adicionar paginação e filtros compostos para consultas de trades.

public interface ITradeRepository
{
    Task AddAsync(Trade trade, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Trade>> ListAsync(string? asset = null, DateTimeOffset? start = null, DateTimeOffset? end = null, Guid? orderId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Trade>> ListPageAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, int page, int pageSize, CancellationToken cancellationToken = default)
        => ListPageFallbackAsync(asset, start, end, orderId, page, pageSize, cancellationToken);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);

    async Task<PagedResult<Trade>> ListPageFallbackAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var all = await ListAsync(asset, start, end, orderId, cancellationToken);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
        return new PagedResult<Trade>(items, page, pageSize, all.Count);
    }
}
