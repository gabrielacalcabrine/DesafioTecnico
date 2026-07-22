using Trading.Domain.Services;
using Trading.Domain.ValueObjects;

namespace Trading.Domain.Entities;

public class Trade
{
    private Trade() { }
    public Trade(Guid buyOrderId, Guid sellOrderId, string asset, int quantity, decimal executionPrice, IClock? clock = null)
    {
        if (buyOrderId == Guid.Empty) throw new ArgumentException("A ordem de compra é obrigatória.", nameof(buyOrderId));
        if (sellOrderId == Guid.Empty) throw new ArgumentException("A ordem de venda é obrigatória.", nameof(sellOrderId));
        if (buyOrderId == sellOrderId) throw new ArgumentException("As ordens de compra e venda devem ser diferentes.", nameof(sellOrderId));
        Id = Guid.NewGuid(); BuyOrderId = buyOrderId; SellOrderId = sellOrderId;
        Asset = new AssetTicker(asset).Value;
        Quantity = new OrderQuantity(quantity).Value;
        ExecutionPrice = new OrderPrice(executionPrice).Value;
        ExecutedAt = (clock ?? new SystemClock()).UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid BuyOrderId { get; private set; }
    public Guid SellOrderId { get; private set; }
    public string Asset { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal ExecutionPrice { get; private set; }
    public DateTimeOffset ExecutedAt { get; private set; }
}
