using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Auth;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Auth.Commands.Login;

public record LoginCommand(LoginDto Credentials) : IRequest<AuthUserDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Credentials.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Credentials.Password).NotEmpty();
    }
}

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService,
    IAuthCookieService authCookieService,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : IRequestHandler<LoginCommand, AuthUserDto>
{
    public async Task<AuthUserDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Credentials.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Неверный email или пароль.");

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Credentials.Password);
        if (verification == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Неверный email или пароль.");

        var token = jwtTokenService.CreateToken(user);
        var response = httpContextAccessor.HttpContext?.Response
            ?? throw new InvalidOperationException("HttpContext недоступен.");

        authCookieService.AppendAccessToken(response, token);

        return mapper.Map<AuthUserDto>(user);
    }
}
