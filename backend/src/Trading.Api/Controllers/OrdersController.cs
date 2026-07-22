using Microsoft.AspNetCore.Mvc;
using Trading.Api.DTOs;
using Trading.Application.Services;
using Trading.Application.Services.Interfaces;

namespace Trading.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController(IOrderService orders) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        var order = await orders.CreateAsync((Trading.Domain.Enums.OrderType)request.Tipo, request.Ativo, request.Quantidade, request.Preco, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponseDto>>> List([FromQuery] OrderQueryDto query, CancellationToken cancellationToken)
    {
        Trading.Domain.Enums.OrderStatus? status = query.Status.HasValue ? (Trading.Domain.Enums.OrderStatus)query.Status.Value : null;
        if (!query.Page.HasValue && !query.PageSize.HasValue)
        {
            var result = await orders.ListAsync(query.Ativo, status, cancellationToken);
            return Ok(result.Select(x => x.ToDto()).ToArray());
        }
        var paged = await orders.ListPageAsync(query.Ativo, status, query.Page ?? 1, query.PageSize ?? 50, cancellationToken);
        return Ok(new { items = paged.Items.Select(x => x.ToDto()).ToArray(), paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order.ToDto());
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok((await orders.CancelAsync(id, cancellationToken)).ToDto()); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException exception) { return Conflict(new ErrorResponseDto { Code = "order_not_cancellable", Message = exception.Message }); }
    }
}
