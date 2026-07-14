using System.Security.Claims;
using AutoMapper;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<AuthUserDto>;

public class GetCurrentUserQueryHandler(
    IUserRepository userRepository,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : IRequestHandler<GetCurrentUserQuery, AuthUserDto>
{
    public async Task<AuthUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userIdValue = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Пользователь не авторизован.");

        if (!Guid.TryParse(userIdValue, out var userId))
            throw new UnauthorizedAccessException("Некорректный идентификатор пользователя.");

        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Пользователь не найден.");

        return mapper.Map<AuthUserDto>(user);
    }
}
