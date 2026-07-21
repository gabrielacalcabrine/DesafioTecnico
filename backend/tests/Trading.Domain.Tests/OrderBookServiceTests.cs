using Trading.Application.Services;
using Trading.Domain.Entities;
using Trading.Domain.Enums;

namespace Trading.Domain.Tests;

public sealed class OrderBookServiceTests
{
    [Fact]
    public async Task ShouldAggregateBuyLevelsDescendingAndSellLevelsAscending()
    {
        var repository = new FakeOrderRepository();
        await repository.AddAsync(new Order(OrderType.Compra, "PETR4", 10, 30m));
        await repository.AddAsync(new Order(OrderType.Compra, "PETR4", 20, 31m));
        await repository.AddAsync(new Order(OrderType.Venda, "PETR4", 15, 32m));

        var book = await new OrderBookService(repository).GetAsync("PETR4");

        Assert.Equal([31m, 30m], book.Buys.Select(x => x.Price));
        Assert.Equal([32m], book.Sells.Select(x => x.Price));
    }

    [Fact]
    public async Task ShouldExposeOnlyRemainingQuantity()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(OrderType.Compra, "PETR4", 100, 30m);
        order.Execute(40);
        await repository.AddAsync(order);

        var book = await new OrderBookService(repository).GetAsync("PETR4");

        Assert.Equal(60, book.Buys.Single().Quantity);
    }
}
