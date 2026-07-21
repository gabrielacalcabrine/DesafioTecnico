using Trading.Domain.Enums;
using Trading.Domain.Events;
using Trading.Domain.Services;
using Trading.Domain.ValueObjects;

namespace Trading.Domain.Entities;

public class Order
{
    private Order() { }
    private readonly List<IDomainEvent> domainEvents = [];

    public Order(OrderType type, string asset, int quantity, decimal price, IClock? clock = null)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        var ticker = new AssetTicker(asset);
        var orderQuantity = new OrderQuantity(quantity);
        var orderPrice = new OrderPrice(price);
        Id = Guid.NewGuid(); Type = type; Asset = ticker.Value; Quantity = orderQuantity.Value; Price = orderPrice.Value;
        Status = OrderStatus.Aberta;
        CreatedAt = (clock ?? new SystemClock()).UtcNow;
        domainEvents.Add(new OrderCreatedEvent(Id, CreatedAt));
    }

    public Guid Id { get; private set; }
    public OrderType Type { get; private set; }
    public string Asset { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public int ExecutedQuantity { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public int RemainingQuantity => Quantity - ExecutedQuantity;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents;

    public void Execute(int quantity)
    {
        if (Status is OrderStatus.Executada or OrderStatus.Cancelada)
            throw new InvalidOperationException("A ordem não pode ser executada.");
        if (quantity <= 0 || quantity > RemainingQuantity) throw new ArgumentOutOfRangeException(nameof(quantity));
        ExecutedQuantity += quantity;
        Status = RemainingQuantity == 0 ? OrderStatus.Executada : OrderStatus.ParcialmenteExecutada;
        domainEvents.Add(new OrderExecutedEvent(Id, quantity, Status, DateTimeOffset.UtcNow));
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Executada or OrderStatus.Cancelada) throw new InvalidOperationException("A ordem não pode ser cancelada.");
        Status = OrderStatus.Cancelada;
        domainEvents.Add(new OrderCancelledEvent(Id, DateTimeOffset.UtcNow));
    }
}
