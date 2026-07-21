using Trading.Domain.Entities;

namespace Trading.Api.DTOs;

// DONE: O mapper possui cobertura por testes unitários.
public static class TradingMapper
{
    public static OrderResponseDto ToDto(this Order order) => new()
    {
        Id = order.Id, Tipo = ToDto(order.Type), Ativo = order.Asset, Quantidade = order.Quantity,
        Preco = order.Price, QuantidadeExecutada = order.ExecutedQuantity,
        Status = ToDto(order.Status), DataHoraCriacao = order.CreatedAt
    };

    public static TradeResponseDto ToDto(this Trade trade) => new()
    {
        Id = trade.Id, OrdemCompraId = trade.BuyOrderId, OrdemVendaId = trade.SellOrderId,
        Ativo = trade.Asset, Quantidade = trade.Quantity, PrecoExecucao = trade.ExecutionPrice,
        DataHoraExecucao = trade.ExecutedAt
    };

    private static OrderType ToDto(Trading.Domain.Enums.OrderType type) => type switch
    {
        Trading.Domain.Enums.OrderType.Compra => OrderType.Compra,
        Trading.Domain.Enums.OrderType.Venda => OrderType.Venda,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tipo de ordem não suportado.")
    };

    private static OrderStatus ToDto(Trading.Domain.Enums.OrderStatus status) => status switch
    {
        Trading.Domain.Enums.OrderStatus.Aberta => OrderStatus.Aberta,
        Trading.Domain.Enums.OrderStatus.ParcialmenteExecutada => OrderStatus.ParcialmenteExecutada,
        Trading.Domain.Enums.OrderStatus.Executada => OrderStatus.Executada,
        Trading.Domain.Enums.OrderStatus.Cancelada => OrderStatus.Cancelada,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Status de ordem não suportado.")
    };
}
