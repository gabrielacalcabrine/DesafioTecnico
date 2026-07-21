namespace Trading.Application.Models;

// DONE: O livro é agregado por nível de preço, conforme permitido pelo contrato da API.

public sealed record OrderBookSnapshot(string Asset, IReadOnlyCollection<OrderBookLevel> Buys, IReadOnlyCollection<OrderBookLevel> Sells);
public sealed record OrderBookLevel(decimal Price, int Quantity);
