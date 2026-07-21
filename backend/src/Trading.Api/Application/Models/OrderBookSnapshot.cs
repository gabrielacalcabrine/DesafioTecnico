namespace Trading.Application.Models;

// TODO: Definir se o livro será agregado por preço ou retornará cada ordem individualmente.

public sealed record OrderBookSnapshot(string Asset, IReadOnlyCollection<OrderBookLevel> Buys, IReadOnlyCollection<OrderBookLevel> Sells);
public sealed record OrderBookLevel(decimal Price, int Quantity);
