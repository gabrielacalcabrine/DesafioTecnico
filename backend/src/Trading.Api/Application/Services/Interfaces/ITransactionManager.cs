namespace Trading.Application.Services.Interfaces;

public interface ITransactionManager
{
    Task ExecuteConsistentAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);
}
