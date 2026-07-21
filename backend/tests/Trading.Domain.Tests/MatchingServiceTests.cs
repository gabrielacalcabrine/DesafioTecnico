using Trading.Application.Services;
using Trading.Domain.Entities;
using Trading.Domain.Enums;

namespace Trading.Domain.Tests;

public sealed class MatchingServiceTests
{
    [Fact]
    public async Task ShouldMatchBuyAgainstBestSellAndCreateTrade()
    {
        var orders = new FakeOrderRepository();
        var trades = new FakeTradeRepository();
        var sell = new Order(OrderType.Venda, "PETR4", 50, 30m);
        var incoming = new Order(OrderType.Compra, "PETR4", 100, 30.50m);
        await orders.AddAsync(sell);
        await orders.AddAsync(incoming);

        await new MatchingService(orders, trades).MatchAsync(incoming);

        Assert.Equal(OrderStatus.ParcialmenteExecutada, incoming.Status);
        Assert.Equal(OrderStatus.Executada, sell.Status);
        Assert.Single(trades.Items);
        Assert.Equal(50, trades.Items[0].Quantity);
        Assert.Equal(30m, trades.Items[0].ExecutionPrice);
    }

    [Fact]
    public async Task ShouldNotMatchDifferentAssets()
    {
        var orders = new FakeOrderRepository();
        var trades = new FakeTradeRepository();
        var sell = new Order(OrderType.Venda, "VALE3", 50, 30m);
        var buy = new Order(OrderType.Compra, "PETR4", 50, 30.50m);
        await orders.AddAsync(sell);
        await orders.AddAsync(buy);

        await new MatchingService(orders, trades).MatchAsync(buy);

        Assert.Empty(trades.Items);
        Assert.Equal(OrderStatus.Aberta, buy.Status);
    }

    [Fact]
    public async Task ShouldRespectPriceCompatibility()
    {
        var orders = new FakeOrderRepository();
        var trades = new FakeTradeRepository();
        var sell = new Order(OrderType.Venda, "PETR4", 50, 31m);
        var buy = new Order(OrderType.Compra, "PETR4", 50, 30m);
        await orders.AddAsync(sell);
        await orders.AddAsync(buy);

        await new MatchingService(orders, trades).MatchAsync(buy);

        Assert.Empty(trades.Items);
    }
}
