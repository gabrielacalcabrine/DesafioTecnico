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
}
