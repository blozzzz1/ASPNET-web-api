using AutoMapper;
using FluentValidation;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, UpdateUserDto User) : IRequest<UserDto>;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.User.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.User.Phone).MaximumLength(32);
    }
}

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Пользователь с Id '{request.Id}' не найден.");

        var byEmail = await userRepository.GetByEmailAsync(request.User.Email, cancellationToken);
        if (byEmail is not null && byEmail.Id != user.Id)
            throw new InvalidOperationException($"Email '{request.User.Email}' уже занят.");

        mapper.Map(request.User, user);
        user.Email = request.User.Email.Trim().ToLowerInvariant();
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<UserDto>(user);
    }
}
