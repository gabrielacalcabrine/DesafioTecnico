namespace Trading.Application.Services.Interfaces;

public interface IAdminService
{
    Task ResetAsync(CancellationToken cancellationToken = default);
}
