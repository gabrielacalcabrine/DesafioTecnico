using Trading.Domain.Entities;

namespace Trading.Application.Services.Interfaces;

public interface IMatchingService
{
    Task MatchAsync(Order incomingOrder, CancellationToken cancellationToken = default);
}
