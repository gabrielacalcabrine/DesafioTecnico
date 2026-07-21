namespace Trading.Domain.Services;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
