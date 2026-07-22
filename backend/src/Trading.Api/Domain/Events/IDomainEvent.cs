namespace Trading.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
