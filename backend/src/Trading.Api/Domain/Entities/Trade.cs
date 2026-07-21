namespace Trading.Domain.Entities;

// DONE: Invariantes básicas de trade são validadas no construtor.
// TODO: Adicionar referências às ordens quando o modelo exigir navegação rica.

public class Trade
{
    private Trade() { }
    public Trade(Guid buyOrderId, Guid sellOrderId, string asset, int quantity, decimal executionPrice)
    {
        if (buyOrderId == Guid.Empty) throw new ArgumentException("A ordem de compra é obrigatória.", nameof(buyOrderId));
        if (sellOrderId == Guid.Empty) throw new ArgumentException("A ordem de venda é obrigatória.", nameof(sellOrderId));
        if (buyOrderId == sellOrderId) throw new ArgumentException("As ordens de compra e venda devem ser diferentes.", nameof(sellOrderId));
        if (string.IsNullOrWhiteSpace(asset)) throw new ArgumentException("Ativo é obrigatório.", nameof(asset));
        if (asset.Trim().Length > 20 || asset.Trim().Any(character => !char.IsLetterOrDigit(character)))
            throw new ArgumentException("Ativo deve conter de 1 a 20 caracteres alfanuméricos.", nameof(asset));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (executionPrice <= 0) throw new ArgumentOutOfRangeException(nameof(executionPrice));
        Id = Guid.NewGuid(); BuyOrderId = buyOrderId; SellOrderId = sellOrderId; Asset = asset.Trim().ToUpperInvariant();
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
