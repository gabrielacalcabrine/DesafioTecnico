using Trading.Domain.Entities;
using Trading.Application.Models;

namespace Trading.Application.Repositories.Interfaces;

// TODO: Adicionar consultas específicas para o matching com prioridade de preço e tempo.

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> ListAsync(string? asset = null, string? status = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Order>> ListPageAsync(string? asset, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
        => ListPageFallbackAsync(asset, status, page, pageSize, cancellationToken);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);

    async Task<PagedResult<Order>> ListPageFallbackAsync(string? asset, string? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        var all = await ListAsync(asset, status, cancellationToken);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
        return new PagedResult<Order>(items, page, pageSize, all.Count);
    }
}
