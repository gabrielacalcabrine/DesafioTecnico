using Trading.Domain.Entities;
using Trading.Application.Models;

namespace Trading.Application.Repositories.Interfaces;

// TODO: Adicionar consultas específicas para o matching com prioridade de preço e tempo.

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> ListAsync(string? asset = null, string? status = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Order>> ListPageAsync(string? asset, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);

}
