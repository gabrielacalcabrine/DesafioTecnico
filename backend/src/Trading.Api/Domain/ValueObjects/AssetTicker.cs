namespace Trading.Domain.ValueObjects;

public readonly record struct AssetTicker
{
    public string Value { get; }

    public AssetTicker(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 20 || value.Trim().Any(char.IsLetterOrDigit) is false)
            throw new ArgumentException("Ativo deve conter de 1 a 20 caracteres alfanuméricos.", nameof(value));

        var normalized = value.Trim();
        if (normalized.Any(character => !char.IsLetterOrDigit(character)))
            throw new ArgumentException("Ativo deve conter de 1 a 20 caracteres alfanuméricos.", nameof(value));

        Value = normalized.ToUpperInvariant();
    }

    public override string ToString() => Value;
    public static implicit operator string(AssetTicker ticker) => ticker.Value;
}
