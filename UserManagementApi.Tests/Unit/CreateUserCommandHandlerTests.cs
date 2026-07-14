using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Application.Common.Mappings;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Users.Commands.CreateUser;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Data;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Tests.Unit;

public class CreateUserCommandHandlerTests
{
    private static (CreateUserCommandHandler Handler, AppDbContext Db) CreateSut()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        var handler = new CreateUserCommandHandler(
            new UserRepository(db),
            new PasswordHasher<User>(),
            mapper);

        return (handler, db);
    }

    [Fact]
    public async Task Handle_CreatesUser_WithHashedPassword()
    {
        var (handler, db) = CreateSut();

        var result = await handler.Handle(new CreateUserCommand(new CreateUserDto(
            "Иван",
            "Тестов",
            "ivan.test@example.com",
            "Password123!",
            null)), CancellationToken.None);

        result.Email.Should().Be("ivan.test@example.com");
        result.Role.Should().Be("User");

        var stored = await db.Users.SingleAsync();
        stored.PasswordHash.Should().NotBeNullOrWhiteSpace();
        stored.PasswordHash.Should().NotBe("Password123!");
    }

    [Fact]
    public async Task Handle_WhenEmailExists_Throws()
    {
        var (handler, _) = CreateSut();
        var dto = new CreateUserDto("A", "B", "dup@example.com", "Password123!", null);

        await handler.Handle(new CreateUserCommand(dto), CancellationToken.None);

        var act = async () => await handler.Handle(new CreateUserCommand(dto), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*уже существует*");
    }
}
