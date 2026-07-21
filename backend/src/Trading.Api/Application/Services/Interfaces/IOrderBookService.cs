using Trading.Application.Models;

namespace Trading.Application.Services.Interfaces;

// TODO: Definir estratégia de atualização do book: polling, eventos ou SignalR.

public interface IOrderBookService
{
    Task<OrderBookSnapshot> GetAsync(string asset, CancellationToken cancellationToken = default);
}
