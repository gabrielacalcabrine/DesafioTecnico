namespace Trading.Application.Services.Interfaces;

// TODO: Proteger o reset por configuração e autorização antes de disponibilizar em produção.

public interface IAdminService
{
    Task ResetAsync(CancellationToken cancellationToken = default);
}
