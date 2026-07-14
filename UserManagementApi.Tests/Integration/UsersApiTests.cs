using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Tests.Integration;

public class UsersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersApiTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GetUsers_WithoutAuth_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_WithAuth_ReturnsSeedUser()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().Contain(u => u.Email == "ivan.petrov@example.com");
    }

    [Fact]
    public async Task CreateUser_WithAuth_ReturnsCreated()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var email = $"created_{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/users", new
        {
            firstName = "Анна",
            lastName = "Смирнова",
            email,
            password = "Password123!",
            phone = "+79007654321"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user!.Email.Should().Be(email);
        user.Role.Should().Be("User");
    }
}
