namespace Trading.Application.Services.Interfaces;

public interface ITransactionManager
{
    Task ExecuteSerializableAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);
}
