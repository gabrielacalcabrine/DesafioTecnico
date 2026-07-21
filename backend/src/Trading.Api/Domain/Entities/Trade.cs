namespace Trading.Domain.Entities;

// TODO: Validar invariantes de trade e adicionar referência às ordens quando necessário.

public class Trade
{
    private Trade() { }
    public Trade(Guid buyOrderId, Guid sellOrderId, string asset, int quantity, decimal executionPrice)
    {
        Id = Guid.NewGuid(); BuyOrderId = buyOrderId; SellOrderId = sellOrderId; Asset = asset;
        Quantity = quantity; ExecutionPrice = executionPrice; ExecutedAt = DateTimeOffset.UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid BuyOrderId { get; private set; }
    public Guid SellOrderId { get; private set; }
    public string Asset { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal ExecutionPrice { get; private set; }
    public DateTimeOffset ExecutedAt { get; private set; }
}
