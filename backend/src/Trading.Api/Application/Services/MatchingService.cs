using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Entities;
using Trading.Domain.Enums;
using Trading.Application.Events;
using System.Collections.Concurrent;
namespace Trading.Application.Services;

// DONE: Execução parcial e prioridade preço-tempo possuem implementação e testes básicos.
public sealed class MatchingService(
    IOrderRepository orders,
    ITradeRepository trades,
    ITransactionManager? transactionManager = null,
    ILogger<MatchingService>? logger = null,
    IDomainEventPublisher? eventPublisher = null) : IMatchingService
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> AssetLocks = new(StringComparer.OrdinalIgnoreCase);

    public async Task MatchAsync(Order incomingOrder, CancellationToken cancellationToken = default)
    {
        var assetLock = AssetLocks.GetOrAdd(incomingOrder.Asset, _ => new SemaphoreSlim(1, 1));
        await assetLock.WaitAsync(cancellationToken);
        try
        {
            if (transactionManager is not null)
            {
                await transactionManager.ExecuteConsistentAsync(
                    ct => MatchCoreAsync(incomingOrder, ct),
                    cancellationToken);
                return;
            }

            await MatchCoreAsync(incomingOrder, cancellationToken);
        }
        finally
        {
            assetLock.Release();
        }
    }

    private async Task MatchCoreAsync(Order incomingOrder, CancellationToken cancellationToken)
    {
        var active = await orders.ListAsync(incomingOrder.Asset, cancellationToken: cancellationToken);
        var candidates = active.Where(x => x.Id != incomingOrder.Id && x.RemainingQuantity > 0 
        && x.Status is OrderStatus.Aberta or OrderStatus.ParcialmenteExecutada)
            .Where(x => incomingOrder.Type == OrderType.Compra ?
            x.Type == OrderType.Venda && incomingOrder.Price >= x.Price : x.Type == OrderType.Compra && incomingOrder.Price <= x.Price)
            .OrderBy(x => incomingOrder.Type == OrderType.Compra ? x.Price : -x.Price).ThenBy(x => x.CreatedAt).ToArray();
        foreach (var counterOrder in candidates)
        {
            var quantity = Math.Min(incomingOrder.RemainingQuantity, counterOrder.RemainingQuantity);
            if (quantity <= 0) break;
            incomingOrder.Execute(quantity); counterOrder.Execute(quantity);
            await orders.AddAsync(counterOrder, cancellationToken); await orders.AddAsync(incomingOrder, cancellationToken);
            var buy = incomingOrder.Type == OrderType.Compra ? incomingOrder : counterOrder;
            var sell = incomingOrder.Type == OrderType.Venda ? incomingOrder : counterOrder;
            var executionPrice = sell.Price;
            await trades.AddAsync(new Trade(buy.Id, sell.Id, incomingOrder.Asset, quantity, executionPrice), cancellationToken);
            if (eventPublisher is not null)
            {
                await eventPublisher.PublishAsync(buy.DomainEvents.Concat(sell.DomainEvents), cancellationToken);
            }
            logger?.LogInformation("Trade executed for {Asset}: {Quantity} units at {Price}", incomingOrder.Asset, quantity, executionPrice);
        }
    }
}
