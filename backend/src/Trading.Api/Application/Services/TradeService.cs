using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Entities;
namespace Trading.Application.Services;

// TODO: Adicionar paginação, ordenação e validação dos filtros recebidos.
public sealed class TradeService(ITradeRepository tradeRepository) : ITradeService
{
    public Task<IReadOnlyCollection<Trade>> ListAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, CancellationToken cancellationToken = default) => tradeRepository.ListAsync(asset, start, end, orderId, cancellationToken);
}
