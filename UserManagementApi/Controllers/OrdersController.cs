using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Orders.Commands.CreateOrder;
using UserManagementApi.Application.Orders.Commands.DeleteOrder;
using UserManagementApi.Application.Orders.Commands.UpdateOrderStatus;
using UserManagementApi.Application.Orders.Queries.GetOrderById;
using UserManagementApi.Application.Orders.Queries.GetOrders;

namespace UserManagementApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetOrdersQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetOrderByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateOrderCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
        => Ok(await mediator.Send(new UpdateOrderStatusCommand(id, dto), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOrderCommand(id), cancellationToken);
        return NoContent();
    }
}
