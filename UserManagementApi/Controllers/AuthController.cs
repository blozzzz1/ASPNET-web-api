using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Application.Auth.Commands.Login;
using UserManagementApi.Application.Auth.Commands.Logout;
using UserManagementApi.Application.Auth.Commands.Register;
using UserManagementApi.Application.Auth.Queries.GetCurrentUser;
using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthUserDto>> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new RegisterCommand(dto), cancellationToken);
        return Created("/api/auth/me", user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthUserDto>> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new LoginCommand(dto), cancellationToken));

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutCommand(), cancellationToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthUserDto>> Me(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetCurrentUserQuery(), cancellationToken));
}
