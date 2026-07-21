using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Entities;
using Trading.Domain.Enums;
namespace Trading.Application.Services;

// TODO: Substituir exceções genéricas por erros de aplicação tipados e validar o comando antes do domínio.
public sealed class OrderService(IOrderRepository orderRepository, IMatchingService matchingService) : IOrderService
{
    public async Task<Order> CreateAsync(OrderType type, string asset, int quantity, decimal price, CancellationToken cancellationToken = default) { var order = new Order(type, asset, quantity, price); await orderRepository.AddAsync(order, cancellationToken); await matchingService.MatchAsync(order, cancellationToken); return order; }
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => orderRepository.GetByIdAsync(id, cancellationToken);
    public Task<IReadOnlyCollection<Order>> ListAsync(string? asset, OrderStatus? status, CancellationToken cancellationToken = default) => orderRepository.ListAsync(asset, status?.ToString(), cancellationToken);
    public async Task<Order> CancelAsync(Guid id, CancellationToken cancellationToken = default) { var order = await orderRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Ordem não encontrada."); order.Cancel(); await orderRepository.AddAsync(order, cancellationToken); return order; }
}
