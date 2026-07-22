using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Entities;
using Trading.Domain.Enums;
using Trading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Trading.Application.Events;
namespace Trading.Application.Services;

// DONE: Execução parcial e prioridade preço-tempo possuem implementação e testes básicos.
// TODO: Cobrir concorrência e garantir atomicidade com uma transação.
public sealed class MatchingService(IOrderRepository orders, ITradeRepository trades, TradingDbContext? db = null, ILogger<MatchingService>? logger = null, IDomainEventPublisher? eventPublisher = null) : IMatchingService
{
    public async Task MatchAsync(Order incomingOrder, CancellationToken cancellationToken = default)
    {
        if (db is not null)
        {
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => await MatchCoreAsync(incomingOrder, cancellationToken));
            return;
        }
        await MatchCoreAsync(incomingOrder, cancellationToken);
    }

    private async Task MatchCoreAsync(Order incomingOrder, CancellationToken cancellationToken)
    {
        await using var transaction = db is null ? null : await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
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
            await trades.AddAsync(new Trade(buy.Id, sell.Id, incomingOrder.Asset, quantity, counterOrder.Price), cancellationToken);
            if (eventPublisher is not null)
            {
                await eventPublisher.PublishAsync(buy.DomainEvents.Concat(sell.DomainEvents), cancellationToken);
            }
            logger?.LogInformation("Trade executed for {Asset}: {Quantity} units at {Price}", incomingOrder.Asset, quantity, counterOrder.Price);
        }
        if (transaction is not null) await transaction.CommitAsync(cancellationToken);
    }
}
