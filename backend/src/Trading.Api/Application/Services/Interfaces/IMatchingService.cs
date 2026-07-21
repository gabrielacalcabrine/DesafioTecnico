using Trading.Domain.Entities;

namespace Trading.Application.Services.Interfaces;

// TODO: Definir uma operação transacional para impedir matching duplicado em requisições concorrentes.

public interface IMatchingService
{
    Task MatchAsync(Order incomingOrder, CancellationToken cancellationToken = default);
}
