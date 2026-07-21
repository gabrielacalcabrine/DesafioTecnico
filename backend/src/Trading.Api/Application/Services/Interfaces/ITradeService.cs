using Trading.Domain.Entities;

namespace Trading.Application.Services.Interfaces;

// TODO: Adicionar regras de intervalo de datas e paginação ao contrato de consulta.

public interface ITradeService
{
    Task<IReadOnlyCollection<Trade>> ListAsync(string? asset, DateTimeOffset? start, DateTimeOffset? end, Guid? orderId, CancellationToken cancellationToken = default);
}
