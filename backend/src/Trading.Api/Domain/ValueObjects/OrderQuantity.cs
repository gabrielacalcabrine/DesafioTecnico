namespace Trading.Domain.ValueObjects;

public readonly record struct OrderQuantity
{
    public int Value { get; }
    public OrderQuantity(int value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        Value = value;
    }
    public static implicit operator int(OrderQuantity quantity) => quantity.Value;
}
