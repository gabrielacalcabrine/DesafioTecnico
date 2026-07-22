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
        if (!query.Page.HasValue && !query.PageSize.HasValue)
        {
            var result = await trades.ListAsync(query.Ativo, query.Inicio, query.Fim, query.OrdemId, cancellationToken);
            return Ok(result.Select(x => x.ToDto()).ToArray());
        }
        var paged = await trades.ListPageAsync(query.Ativo, query.Inicio, query.Fim, query.OrdemId, query.Page ?? 1, query.PageSize ?? 50, cancellationToken);
        return Ok(new { items = paged.Items.Select(x => x.ToDto()).ToArray(), paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages });
    }
}
