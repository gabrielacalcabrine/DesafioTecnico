using Trading.Application.Services;
using Trading.Domain.Entities;

namespace Trading.Domain.Tests;

public sealed class TradeServiceTests
{
    [Fact]
    public async Task ShouldReturnTradesUsingRepositoryFilters()
    {
        var repository = new FakeTradeRepository();
        var trade = new Trade(Guid.NewGuid(), Guid.NewGuid(), "PETR4", 10, 30m);
        await repository.AddAsync(trade);
        var result = await new TradeService(repository).ListAsync("PETR4", null, null, trade.BuyOrderId);
        Assert.Single(result);
        Assert.Equal(trade.Id, result.Single().Id);
    }
}
