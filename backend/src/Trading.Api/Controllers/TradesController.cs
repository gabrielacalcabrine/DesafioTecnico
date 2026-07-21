using Microsoft.AspNetCore.Mvc;
using Trading.Api.DTOs;
using Trading.Application.Services;
using Trading.Application.Services.Interfaces;

namespace Trading.Api.Controllers;

// TODO: Adicionar paginação e limite máximo para evitar respostas muito grandes.

[ApiController]
[Route("trades")]
public sealed class TradesController(ITradeService trades) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TradeResponseDto>>> List([FromQuery] TradeQueryDto query, CancellationToken cancellationToken)
    {
        var result = await trades.ListAsync(query.Ativo, query.Inicio, query.Fim, query.OrdemId, cancellationToken);
        return Ok(result.Select(x => x.ToDto()).ToArray());
    }
}
