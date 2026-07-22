using Trading.Domain.Entities;
using Trading.Domain.Enums;

namespace Trading.Domain.Tests;

public sealed class OrderTests
{
    [Fact]
    public void ShouldCreateOpenOrder()
    {
        var order = new Order(OrderType.Compra, "petr4", 100, 30.50m);
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal("PETR4", order.Asset);
        Assert.Equal(OrderStatus.Aberta, order.Status);
        Assert.Equal(100, order.RemainingQuantity);
    }

    [Theory]
    [InlineData(0, 30.50)]
    [InlineData(-1, 30.50)]
    public void ShouldRejectInvalidQuantity(int quantity, decimal price)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Order(OrderType.Compra, "PETR4", quantity, price));

    [Fact]
    public void ShouldRejectInvalidPrice()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Order(OrderType.Compra, "PETR4", 10, 0));

    [Fact]
    public void ShouldSupportPartialExecution()
    {
        var order = new Order(OrderType.Compra, "PETR4", 100, 30.50m);
        order.Execute(40);
        Assert.Equal(40, order.ExecutedQuantity);
        Assert.Equal(60, order.RemainingQuantity);
        Assert.Equal(OrderStatus.ParcialmenteExecutada, order.Status);
    }

    [Fact]
    public void ShouldMarkOrderAsExecutedWhenFullyFilled()
    {
        var order = new Order(OrderType.Compra, "PETR4", 100, 30.50m);
        order.Execute(100);
        Assert.Equal(OrderStatus.Executada, order.Status);
    }

    [Fact]
    public void ShouldCancelOpenOrder()
    {
        var order = new Order(OrderType.Venda, "VALE3", 10, 60m);
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelada, order.Status);
    }

    [Fact]
    public void ShouldNotCancelExecutedOrder()
    {
        var order = new Order(OrderType.Compra, "PETR4", 10, 30m);
        order.Execute(10);
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void ShouldNotCancelAlreadyCancelledOrder()
    {
        var order = new Order(OrderType.Venda, "VALE3", 10, 60m);
        order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void ShouldRejectInvalidAssetAndExecutionState()
    {
        Assert.Throws<ArgumentException>(() => new Order(OrderType.Compra, "PETR 4", 1, 1m));
        var order = new Order(OrderType.Compra, "PETR4", 10, 1m);
        Assert.Throws<ArgumentOutOfRangeException>(() => order.Execute(11));
        order.Cancel();
        Assert.Throws<InvalidOperationException>(() => order.Execute(1));
    }

    [Fact]
    public void ShouldExposeCreationEvent()
    {
        var order = new Order(OrderType.Compra, "PETR4", 1, 1m);
        Assert.Single(order.DomainEvents);
    }
}
