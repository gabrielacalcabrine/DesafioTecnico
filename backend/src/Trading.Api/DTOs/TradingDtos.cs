using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Trading.Api.DTOs;

// TODO: Separar DTOs por recurso e adicionar validações de negócio que não cabem em DataAnnotations.

public enum OrderType
{
    Compra,
    Venda
}

public enum OrderStatus
{
    Aberta,
    ParcialmenteExecutada,
    Executada,
    Cancelada
}

public sealed class CreateOrderRequestDto : IValidatableObject
{
    [Required]
    [EnumDataType(typeof(OrderType))]
    public OrderType Tipo { get; init; }

    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Ativo { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantidade { get; init; }

    public decimal Preco { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enum.IsDefined(Tipo))
            yield return new ValidationResult("Tipo inválido. Use \"Compra\" ou \"Venda\".", [nameof(Tipo)]);

        if (!TradingValidation.IsValidAsset(Ativo))
            yield return new ValidationResult("Ativo inválido. Use de 1 a 20 caracteres alfanuméricos, por exemplo: PETR4.", [nameof(Ativo)]);

        if (Quantidade <= 0)
            yield return new ValidationResult("A quantidade deve ser maior que zero.", [nameof(Quantidade)]);

        if (Preco <= 0)
            yield return new ValidationResult("O preço deve ser maior que zero.", [nameof(Preco)]);
    }
}

public sealed class OrderResponseDto
{
    public Guid Id { get; init; }
    public OrderType Tipo { get; init; }
    public string Ativo { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal Preco { get; init; }
    public int QuantidadeExecutada { get; init; }
    public OrderStatus Status { get; init; }
    public DateTimeOffset DataHoraCriacao { get; init; }
}

public sealed class OrderBookResponseDto
{
    public string Ativo { get; init; } = string.Empty;
    public IReadOnlyCollection<OrderBookLevelDto> Compras { get; init; } = [];
    public IReadOnlyCollection<OrderBookLevelDto> Vendas { get; init; } = [];
}

public sealed class OrderBookLevelDto
{
    public decimal Preco { get; init; }
    public int Quantidade { get; init; }
}

public sealed class TradeResponseDto
{
    public Guid Id { get; init; }
    public Guid OrdemCompraId { get; init; }
    public Guid OrdemVendaId { get; init; }
    public string Ativo { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoExecucao { get; init; }
    public DateTimeOffset DataHoraExecucao { get; init; }
}

public sealed class OrderQueryDto : IValidatableObject
{
    public string? Ativo { get; init; }
    public OrderStatus? Status { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Ativo is not null && !TradingValidation.IsValidAsset(Ativo))
            yield return new ValidationResult("Ativo inválido. Use de 1 a 20 caracteres alfanuméricos, por exemplo: PETR4.", [nameof(Ativo)]);
    }
}

public sealed class TradeQueryDto : IValidatableObject
{
    public string? Ativo { get; init; }
    public DateTimeOffset? Inicio { get; init; }
    public DateTimeOffset? Fim { get; init; }
    public Guid? OrdemId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Ativo is not null && !TradingValidation.IsValidAsset(Ativo))
            yield return new ValidationResult("Ativo inválido. Use de 1 a 20 caracteres alfanuméricos, por exemplo: PETR4.", [nameof(Ativo)]);

        if (Inicio.HasValue && Fim.HasValue && Inicio > Fim)
            yield return new ValidationResult("O parâmetro inicio deve ser anterior ou igual ao parâmetro fim.", [nameof(Inicio), nameof(Fim)]);
    }
}

public sealed class ErrorResponseDto
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public IReadOnlyCollection<string> Errors { get; init; } = [];
}

internal static partial class TradingValidation
{
    public static bool IsValidAsset(string? asset)
        => !string.IsNullOrWhiteSpace(asset)
           && asset.Trim().Length <= 20
           && AssetPattern().IsMatch(asset.Trim());

    [GeneratedRegex("^[A-Za-z0-9]+$", RegexOptions.CultureInvariant)]
    private static partial Regex AssetPattern();
}
