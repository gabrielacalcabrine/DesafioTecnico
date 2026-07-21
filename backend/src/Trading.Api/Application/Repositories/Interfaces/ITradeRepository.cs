using Trading.Domain.Entities;

namespace Trading.Application.Repositories.Interfaces;

// TODO: Adicionar paginação e filtros compostos para consultas de trades.

public interface ITradeRepository
{
    Task AddAsync(Trade trade, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Trade>> ListAsync(string? asset = null, DateTimeOffset? start = null, DateTimeOffset? end = null, Guid? orderId = null, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}
