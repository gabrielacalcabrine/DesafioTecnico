using Trading.Domain.Entities;
using Trading.Application.Models;

namespace Trading.Application.Services.Interfaces;

public interface ITradeService
{
    Task<IReadOnlyCollection<Trade>> ListAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, CancellationToken cancellationToken = default);
    Task<PagedResult<Trade>> ListPageAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, int page, int pageSize, CancellationToken cancellationToken = default);
}
