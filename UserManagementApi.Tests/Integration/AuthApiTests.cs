using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Tests.Integration;

public class AuthApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthApiTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_WithSeedUser_SetsCookieAndReturnsUser()
    {
        var client = _factory.CreateClient(new() { HandleCookies = true });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "ivan.petrov@example.com",
            password = "Password123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(c => c.StartsWith("access_token="));

        var user = await response.Content.ReadFromJsonAsync<AuthUserDto>();
        user.Should().NotBeNull();
        user!.Email.Should().Be("ivan.petrov@example.com");
        user.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "ivan.petrov@example.com",
            password = "wrong-password"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithoutAuth_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_AfterLogin_ReturnsCurrentUser()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<AuthUserDto>();
        user!.Email.Should().Be("ivan.petrov@example.com");
    }

    [Fact]
    public async Task Register_CreatesUser_AndAllowsLogin()
    {
        var client = _factory.CreateClient(new() { HandleCookies = true });
        var email = $"user_{Guid.NewGuid():N}@example.com";

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Тест",
            lastName = "Пользователь",
            email,
            password = "Password123!",
            phone = "+79001112233"
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "Password123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_ClearsAccessAndBlocksMe()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var logoutResponse = await client.PostAsync("/api/auth/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var meResponse = await client.GetAsync("/api/auth/me");
        meResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
