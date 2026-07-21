using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Entities;
using Trading.Domain.Enums;
using Trading.Application.Models;
using Trading.Application.Events;
namespace Trading.Application.Services;

// TODO: Substituir exceções genéricas por erros de aplicação tipados e validar o comando antes do domínio.
public sealed class OrderService(IOrderRepository orderRepository, IMatchingService matchingService, IDomainEventPublisher? eventPublisher = null) : IOrderService
{
    public async Task<Order> CreateAsync(OrderType type, string asset, int quantity, decimal price, CancellationToken cancellationToken = default) 
    { 
        var order = new Order(type, asset, quantity, price); 
        await orderRepository.AddAsync(order, cancellationToken); 
        if (eventPublisher is not null) await eventPublisher.PublishAsync(order.DomainEvents, cancellationToken);
        await matchingService.MatchAsync(order, cancellationToken); 
        return order; 
    }
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => orderRepository.GetByIdAsync(id, cancellationToken);
    public Task<IReadOnlyCollection<Order>> ListAsync(string? asset, OrderStatus? status, CancellationToken cancellationToken = default) => orderRepository.ListAsync(asset, status?.ToString(), cancellationToken);
    public Task<PagedResult<Order>> ListPageAsync(string? asset, OrderStatus? status, int page, int pageSize, CancellationToken cancellationToken = default) => orderRepository.ListPageAsync(asset, status?.ToString(), page, pageSize, cancellationToken);
    public async Task<Order> CancelAsync(Guid id, CancellationToken cancellationToken = default) 
    { 
        var order = await orderRepository.GetByIdAsync(id, cancellationToken) 
            ?? throw new KeyNotFoundException("Ordem não encontrada."); 
        order.Cancel(); await orderRepository.AddAsync(order, cancellationToken);
        if (eventPublisher is not null) await eventPublisher.PublishAsync(order.DomainEvents, cancellationToken);
        return order;
    }
}
