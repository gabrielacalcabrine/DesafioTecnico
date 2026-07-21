namespace Trading.Domain.ValueObjects;

public readonly record struct OrderPrice
{
    public decimal Value { get; }
    public OrderPrice(decimal value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        Value = value;
    }
    public static implicit operator decimal(OrderPrice price) => price.Value;
    public override string ToString() => Value.ToString();
}
