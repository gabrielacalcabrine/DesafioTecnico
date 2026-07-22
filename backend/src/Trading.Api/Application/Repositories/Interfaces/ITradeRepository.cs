using Trading.Domain.Entities;
using Trading.Application.Models;

namespace Trading.Application.Repositories.Interfaces;

public interface ITradeRepository
{
    Task AddAsync(Trade trade, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Trade>> ListAsync(string? asset = null, DateTimeOffset? start = null, DateTimeOffset? end = null, Guid? orderId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Trade>> ListPageAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);

}
