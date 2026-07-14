using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Users.Commands.CreateUser;
using UserManagementApi.Application.Users.Commands.DeleteUser;
using UserManagementApi.Application.Users.Commands.UpdateUser;
using UserManagementApi.Application.Users.Queries.GetUserById;
using UserManagementApi.Application.Users.Queries.GetUsers;

namespace UserManagementApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetUsersQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetUserByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateUserCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new UpdateUserCommand(id, dto), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return NoContent();
    }
}
