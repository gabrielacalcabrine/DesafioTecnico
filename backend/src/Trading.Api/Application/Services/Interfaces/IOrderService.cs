using Trading.Domain.Entities;
using Trading.Domain.Enums;
using Trading.Application.Models;

namespace Trading.Application.Services.Interfaces;

// TODO: Adicionar operações de validação e consulta paginada de ordens.

public interface IOrderService
{
    Task<Order> CreateAsync(OrderType type, string asset, int quantity, decimal price, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> ListAsync(string? asset, OrderStatus? status, CancellationToken cancellationToken = default);
    Task<PagedResult<Order>> ListPageAsync(string? asset, OrderStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Order> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
