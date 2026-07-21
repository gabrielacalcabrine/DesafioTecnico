using Trading.Domain.Events;

namespace Trading.Application.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}
