using Microsoft.AspNetCore.Mvc;
using Trading.Api.DTOs;
using Trading.Application.Services;
using Trading.Application.Services.Interfaces;

namespace Trading.Api.Controllers;

// TODO: Adicionar cache ou atualização em tempo real para o livro de ofertas.

[ApiController]
[Route("orderbook")]
public sealed class OrderBookController(IOrderBookService orderBook) : ControllerBase
{
    [HttpGet("{asset}")]
    public async Task<IActionResult> Get(string asset, CancellationToken cancellationToken)
    {
        if (!TradingValidation.IsValidAsset(asset))
            return StatusCode(StatusCodes.Status406NotAcceptable, new ErrorResponseDto
            {
                Code = "validation_error",
                Message = "O ativo informado é inválido.",
                Errors = ["Use de 1 a 20 caracteres alfanuméricos, por exemplo: PETR4."]
            });

        var book = await orderBook.GetAsync(asset, cancellationToken);
        return Ok(new OrderBookResponseDto
        {
            Ativo = book.Asset,
            Compras = book.Buys.Select(x => new OrderBookLevelDto { Preco = x.Price, Quantidade = x.Quantity }).ToArray(),
            Vendas = book.Sells.Select(x => new OrderBookLevelDto { Preco = x.Price, Quantidade = x.Quantity }).ToArray()
        });
    }
}
