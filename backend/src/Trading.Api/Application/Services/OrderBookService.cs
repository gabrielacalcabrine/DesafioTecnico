using Trading.Application.Models;
using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
using Trading.Domain.Enums;
namespace Trading.Application.Services;

// TODO: Garantir que a leitura do book use uma consulta consistente durante o matching.
public sealed class OrderBookService(IOrderRepository orders) : IOrderBookService
{
    public async Task<OrderBookSnapshot> GetAsync(string asset, CancellationToken cancellationToken = default)
    {
        var openOrders = (await orders.ListAsync(asset, cancellationToken: cancellationToken)).Where(x => x.RemainingQuantity > 0 && x.Status is OrderStatus.Aberta or OrderStatus.ParcialmenteExecutada);
        var buys = openOrders.Where(x => x.Type == OrderType.Compra).GroupBy(x => x.Price).OrderByDescending(x => x.Key).Select(x => new OrderBookLevel(x.Key, x.Sum(y => y.RemainingQuantity))).ToArray();
        var sells = openOrders.Where(x => x.Type == OrderType.Venda).GroupBy(x => x.Price).OrderBy(x => x.Key).Select(x => new OrderBookLevel(x.Key, x.Sum(y => y.RemainingQuantity))).ToArray();
        return new OrderBookSnapshot(asset.ToUpperInvariant(), buys, sells);
    }
}
