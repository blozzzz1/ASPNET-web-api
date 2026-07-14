using FluentAssertions;
using FluentValidation.TestHelper;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Auth.Commands.Login;
using UserManagementApi.Application.Auth.Commands.Register;

namespace UserManagementApi.Tests.Unit;

public class AuthValidatorsTests
{
    [Fact]
    public void LoginValidator_RejectsEmptyPassword()
    {
        var validator = new LoginCommandValidator();

        var result = validator.TestValidate(new LoginCommand(new LoginDto("a@b.com", "")));

        result.ShouldHaveValidationErrorFor(x => x.Credentials.Password);
    }

    [Fact]
    public void RegisterValidator_RejectsShortPassword()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.TestValidate(new RegisterCommand(new RegisterDto(
            "Иван", "Иванов", "ivan@example.com", "123", null)));

        result.ShouldHaveValidationErrorFor(x => x.User.Password);
    }

    [Fact]
    public void RegisterValidator_AcceptsValidData()
    {
        var validator = new RegisterCommandValidator();

        var result = validator.TestValidate(new RegisterCommand(new RegisterDto(
            "Иван", "Иванов", "ivan@example.com", "Password123!", null)));

        result.ShouldNotHaveAnyValidationErrors();
    }
}
