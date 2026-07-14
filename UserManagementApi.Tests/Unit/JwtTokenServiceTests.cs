using FluentAssertions;
using Microsoft.Extensions.Options;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Auth;

namespace UserManagementApi.Tests.Unit;

public class JwtTokenServiceTests
{
    [Fact]
    public void CreateToken_ReturnsJwtString()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            Key = "UnitTestSecretKey_MustBeAtLeast32Chars!",
            ExpirationMinutes = 30,
            CookieName = "access_token"
        });

        var service = new JwtTokenService(options);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Петров",
            Email = "ivan@example.com",
            Role = "Admin"
        };

        var token = service.CreateToken(user);

        token.Should().NotBeNullOrWhiteSpace();
        token.Split('.').Should().HaveCount(3);
    }
}
