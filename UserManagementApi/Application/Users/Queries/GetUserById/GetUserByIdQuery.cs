using AutoMapper;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

public class GetUserByIdQueryHandler(
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Пользователь с Id '{request.Id}' не найден.");

        return mapper.Map<UserDto>(user);
    }
}
