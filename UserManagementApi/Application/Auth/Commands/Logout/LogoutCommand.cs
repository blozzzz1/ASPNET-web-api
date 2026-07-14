using MediatR;
using UserManagementApi.Infrastructure.Auth;

namespace UserManagementApi.Application.Auth.Commands.Logout;

public record LogoutCommand : IRequest;

public class LogoutCommandHandler(
    IAuthCookieService authCookieService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogoutCommand>
{
    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var response = httpContextAccessor.HttpContext?.Response
            ?? throw new InvalidOperationException("HttpContext недоступен.");

        authCookieService.DeleteAccessToken(response);
        return Task.CompletedTask;
    }
}
