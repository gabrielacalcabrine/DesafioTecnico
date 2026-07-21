using Trading.Domain.Entities;

namespace Trading.Api.DTOs;

// DONE: O mapper possui cobertura por testes unitários.
// TODO: Substituir casts entre enums por mapeamento explícito.

public static class TradingMapper
{
    public static OrderResponseDto ToDto(this Order order) => new()
    {
        Id = order.Id, Tipo = (OrderType)order.Type, Ativo = order.Asset, Quantidade = order.Quantity,
        Preco = order.Price, QuantidadeExecutada = order.ExecutedQuantity,
        Status = (OrderStatus)order.Status, DataHoraCriacao = order.CreatedAt
    };

    public static TradeResponseDto ToDto(this Trade trade) => new()
    {
        Id = trade.Id, OrdemCompraId = trade.BuyOrderId, OrdemVendaId = trade.SellOrderId,
        Ativo = trade.Asset, Quantidade = trade.Quantity, PrecoExecucao = trade.ExecutionPrice,
        DataHoraExecucao = trade.ExecutedAt
    };
}
