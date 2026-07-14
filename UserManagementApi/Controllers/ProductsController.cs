using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Products.Commands.CreateProduct;
using UserManagementApi.Application.Products.Commands.DeleteProduct;
using UserManagementApi.Application.Products.Commands.UpdateProduct;
using UserManagementApi.Application.Products.Queries.GetProductById;
using UserManagementApi.Application.Products.Queries.GetProducts;

namespace UserManagementApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetProductsQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateProductCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new UpdateProductCommand(id, dto), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}
