using Trading.Application.Services;
using Trading.Domain.Entities;

namespace Trading.Domain.Tests;

public sealed class AdminServiceTests
{
    [Fact]
    public async Task ShouldClearOrdersAndTrades()
    {
        var orders = new FakeOrderRepository();
        var trades = new FakeTradeRepository();
        await orders.AddAsync(new Order(Trading.Domain.Enums.OrderType.Compra, "PETR4", 1, 1m));
        await trades.AddAsync(new Trade(Guid.NewGuid(), Guid.NewGuid(), "PETR4", 1, 1m));

        await new AdminService(orders, trades).ResetAsync();

        Assert.Empty(orders.Items);
        Assert.Empty(trades.Items);
    }
}
