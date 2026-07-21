using Trading.Domain.Events;

namespace Trading.Application.Events;

public sealed class LoggingDomainEventPublisher(ILogger<LoggingDomainEventPublisher> logger) : IDomainEventPublisher
{
    public Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in events)
            logger.LogInformation("Domain event published: {EventType} at {OccurredAt}", domainEvent.GetType().Name, domainEvent.OccurredAt);
        return Task.CompletedTask;
    }
}
