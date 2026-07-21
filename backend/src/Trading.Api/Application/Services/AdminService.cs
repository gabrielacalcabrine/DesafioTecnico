using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
namespace Trading.Application.Services;

// DONE: O reset limpa as tabelas por meio dos repositórios EF Core.
// TODO: Envolver a limpeza das tabelas em uma transação explícita.
public sealed class AdminService(IOrderRepository orders, ITradeRepository trades) : IAdminService
{
    public async Task ResetAsync(CancellationToken cancellationToken = default) 
    { await orders.DeleteAllAsync(cancellationToken); await trades.DeleteAllAsync(cancellationToken); }
}
