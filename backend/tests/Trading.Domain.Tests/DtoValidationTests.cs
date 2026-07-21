using System.ComponentModel.DataAnnotations;
using Trading.Api.DTOs;

namespace Trading.Domain.Tests;

public sealed class DtoValidationTests
{
    [Fact]
    public void ShouldAcceptValidCreateOrderRequest()
    {
        var request = new CreateOrderRequestDto { Tipo = OrderType.Compra, Ativo = "PETR4", Quantidade = 100, Preco = 30.50m };
        Assert.Empty(Validate(request));
    }

    [Fact]
    public void ShouldRejectInvalidCreateOrderRequest()
    {
        var request = new CreateOrderRequestDto { Tipo = OrderType.Compra, Ativo = "PETR 4", Quantidade = 100, Preco = 30m };
        var fields = Validate(request).SelectMany(x => x.MemberNames).ToHashSet();
        Assert.Contains(nameof(CreateOrderRequestDto.Ativo), fields);

        request = new CreateOrderRequestDto { Tipo = OrderType.Compra, Ativo = "PETR4", Quantidade = 0, Preco = 0m };
        fields = Validate(request).SelectMany(x => x.MemberNames).ToHashSet();
        Assert.Contains(nameof(CreateOrderRequestDto.Quantidade), fields);

        request = new CreateOrderRequestDto { Tipo = OrderType.Compra, Ativo = "PETR4", Quantidade = 100, Preco = 0m };
        fields = Validate(request).SelectMany(x => x.MemberNames).ToHashSet();
        Assert.Contains(nameof(CreateOrderRequestDto.Preco), fields);
    }

    [Fact]
    public void ShouldRejectInvalidTradeDateRange()
    {
        var query = new TradeQueryDto { Inicio = DateTimeOffset.UtcNow, Fim = DateTimeOffset.UtcNow.AddDays(-1) };
        Assert.Contains(Validate(query), x => x.ErrorMessage!.Contains("inicio"));
    }

    private static IReadOnlyCollection<ValidationResult> Validate(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
}
