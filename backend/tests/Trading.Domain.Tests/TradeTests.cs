using Trading.Domain.Entities;

namespace Trading.Domain.Tests;

public sealed class TradeTests
{
    [Fact]
    public void ShouldCreateTradeWithExecutionTimestamp()
    {
        var buyId = Guid.NewGuid();
        var sellId = Guid.NewGuid();
        var trade = new Trade(buyId, sellId, "PETR4", 50, 30m);
        Assert.Equal(buyId, trade.BuyOrderId);
        Assert.Equal(sellId, trade.SellOrderId);
        Assert.Equal("PETR4", trade.Asset);
        Assert.Equal(50, trade.Quantity);
        Assert.Equal(TimeSpan.Zero, trade.ExecutedAt.Offset);
    }

    [Fact]
    public void ShouldRejectInvalidTradeValues()
    {
        var buyId = Guid.NewGuid();
        var sellId = Guid.NewGuid();

        Assert.Throws<ArgumentOutOfRangeException>(() => new Trade(buyId, sellId, "PETR4", 0, 30m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Trade(buyId, sellId, "PETR4", 1, 0m));
        Assert.Throws<ArgumentException>(() => new Trade(buyId, buyId, "PETR4", 1, 30m));
        Assert.Throws<ArgumentException>(() => new Trade(buyId, sellId, "PETR 4", 1, 30m));
    }
}
