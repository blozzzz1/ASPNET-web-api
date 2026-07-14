using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Auth.Commands.Register;

public record RegisterCommand(RegisterDto User) : IRequest<AuthUserDto>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.User.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.User.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.User.Phone).MaximumLength(32);
    }
}

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IMapper mapper) : IRequestHandler<RegisterCommand, AuthUserDto>
{
    public async Task<AuthUserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await userRepository.GetByEmailAsync(request.User.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Пользователь с email '{request.User.Email}' уже существует.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Email = request.User.Email.Trim().ToLowerInvariant(),
            Phone = request.User.Phone,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.User.Password);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<AuthUserDto>(user);
    }
}
