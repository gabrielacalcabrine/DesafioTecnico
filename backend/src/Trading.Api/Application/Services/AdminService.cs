using Trading.Application.Repositories.Interfaces;
using Trading.Application.Services.Interfaces;
namespace Trading.Application.Services;

// TODO: Trocar o reset em memória por limpeza transacional das tabelas do PostgreSQL.
public sealed class AdminService(IOrderRepository orders, ITradeRepository trades) : IAdminService
{
    public async Task ResetAsync(CancellationToken cancellationToken = default) 
    { await orders.DeleteAllAsync(cancellationToken); await trades.DeleteAllAsync(cancellationToken); }
}
