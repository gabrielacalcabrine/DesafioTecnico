using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Entities;
using Trading.Application.Models;
namespace Trading.Application.Services;

public sealed class TradeService(ITradeRepository tradeRepository) : ITradeService
{
    public Task<IReadOnlyCollection<Trade>> ListAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, CancellationToken cancellationToken = default) => tradeRepository.ListAsync(asset, start, end, orderId, cancellationToken);
    public Task<PagedResult<Trade>> ListPageAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, int page, int pageSize, CancellationToken cancellationToken = default) => tradeRepository.ListPageAsync(asset, start, end, orderId, page, pageSize, cancellationToken);
}
