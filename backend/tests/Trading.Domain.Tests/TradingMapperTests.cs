using Trading.Api.DTOs;
using Trading.Domain.Entities;
using Trading.Domain.Enums;
using DomainOrderType = Trading.Domain.Enums.OrderType;
using DomainOrderStatus = Trading.Domain.Enums.OrderStatus;

namespace Trading.Domain.Tests;

public sealed class TradingMapperTests
{
    [Fact]
    public void ShouldMapOrderToResponseDto()
    {
        var order = new Order(DomainOrderType.Compra, "PETR4", 10, 30m);
        var dto = order.ToDto();
        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(Trading.Api.DTOs.OrderType.Compra, dto.Tipo);
        Assert.Equal("PETR4", dto.Ativo);
        Assert.Equal(Trading.Api.DTOs.OrderStatus.Aberta, dto.Status);
    }

    [Fact]
    public void ShouldMapTradeToResponseDto()
    {
        var trade = new Trade(Guid.NewGuid(), Guid.NewGuid(), "PETR4", 5, 30m);
        var dto = trade.ToDto();
        Assert.Equal(trade.Id, dto.Id);
        Assert.Equal(trade.BuyOrderId, dto.OrdemCompraId);
        Assert.Equal(trade.SellOrderId, dto.OrdemVendaId);
    }
}
