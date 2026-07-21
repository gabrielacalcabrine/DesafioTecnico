using Trading.Application.Services;
using Trading.Domain.Enums;

namespace Trading.Domain.Tests;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task ShouldCreateAndPersistOrderAndCallMatching()
    {
        var repository = new FakeOrderRepository();
        var matching = new FakeMatchingService();
        var order = await new OrderService(repository, matching).CreateAsync(OrderType.Compra, "PETR4", 10, 30m);
        Assert.Contains(order, repository.Items);
        Assert.Contains(order, matching.MatchedOrders);
    }

    [Fact]
    public async Task ShouldCancelExistingOrder()
    {
        var repository = new FakeOrderRepository();
        var order = new Trading.Domain.Entities.Order(OrderType.Compra, "PETR4", 10, 30m);
        await repository.AddAsync(order);
        var result = await new OrderService(repository, new FakeMatchingService()).CancelAsync(order.Id);
        Assert.Equal(OrderStatus.Cancelada, result.Status);
    }

    [Fact]
    public async Task ShouldThrowWhenCancellingUnknownOrder()
    {
        var service = new OrderService(new FakeOrderRepository(), new FakeMatchingService());
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CancelAsync(Guid.NewGuid()));
    }
}
