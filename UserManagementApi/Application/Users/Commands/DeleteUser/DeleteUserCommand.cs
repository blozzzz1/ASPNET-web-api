using MediatR;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;

public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Пользователь с Id '{request.Id}' не найден.");

        userRepository.Remove(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
