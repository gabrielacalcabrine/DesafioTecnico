using Trading.Domain.Enums;

namespace Trading.Domain.Entities;

// TODO: Extrair ativo, preço e quantidade para value objects com invariantes próprias.

public class Order
{
    private Order() { }
    public Order(OrderType type, string asset, int quantity, decimal price)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (string.IsNullOrWhiteSpace(asset) || asset.Trim().Length > 20 || asset.Trim().Any(character => !char.IsLetterOrDigit(character)))
            throw new ArgumentException("Ativo deve conter de 1 a 20 caracteres alfanuméricos.", nameof(asset));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));
        Id = Guid.NewGuid(); Type = type; Asset = asset.Trim().ToUpperInvariant(); Quantity = quantity; Price = price;
        Status = OrderStatus.Aberta; CreatedAt = DateTimeOffset.UtcNow;
    }
    public Guid Id { get; private set; }
    public OrderType Type { get; private set; }
    public string Asset { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public int ExecutedQuantity { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public int RemainingQuantity => Quantity - ExecutedQuantity;
    public void Execute(int quantity)
    {
        if (quantity <= 0 || quantity > RemainingQuantity) throw new ArgumentOutOfRangeException(nameof(quantity));
        ExecutedQuantity += quantity;
        Status = RemainingQuantity == 0 ? OrderStatus.Executada : OrderStatus.ParcialmenteExecutada;
    }
    public void Cancel()
    {
        if (Status is OrderStatus.Executada or OrderStatus.Cancelada) throw new InvalidOperationException("A ordem não pode ser cancelada.");
        Status = OrderStatus.Cancelada;
    }
}
