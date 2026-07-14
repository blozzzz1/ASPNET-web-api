using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Users.Commands.CreateUser;

public record CreateUserCommand(CreateUserDto User) : IRequest<UserDto>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.User.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.User.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.User.Phone).MaximumLength(32);
    }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IMapper mapper) : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.User.Email.Trim().ToLowerInvariant();
        var existing = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Пользователь с email '{request.User.Email}' уже существует.");

        var user = mapper.Map<User>(request.User);
        user.Id = Guid.NewGuid();
        user.Email = email;
        user.Role = "User";
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = passwordHasher.HashPassword(user, request.User.Password);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<UserDto>(user);
    }
}
