using Trading.Domain.Enums;

namespace Trading.Domain.Events;

public sealed record OrderCreatedEvent(Guid OrderId, DateTimeOffset OccurredAt) : IDomainEvent;
public sealed record OrderExecutedEvent(Guid OrderId, int Quantity, OrderStatus Status, DateTimeOffset OccurredAt) : IDomainEvent;
public sealed record OrderCancelledEvent(Guid OrderId, DateTimeOffset OccurredAt) : IDomainEvent;
