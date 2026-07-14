using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Application.Common.Mappings;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Orders.Commands.CreateOrder;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Data;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Tests.Unit;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesOrder_AndDecreasesStock()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new AppDbContext(options);

        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Id = userId,
            FirstName = "A",
            LastName = "B",
            Email = "a@b.com",
            Role = "User"
        };
        user.PasswordHash = hasher.HashPassword(user, "Password123!");

        db.Users.Add(user);
        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Товар",
            Price = 100m,
            StockQuantity = 5,
            IsActive = true
        });
        await db.SaveChangesAsync();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        var handler = new CreateOrderCommandHandler(
            new OrderRepository(db),
            new UserRepository(db),
            new ProductRepository(db),
            mapper);

        var result = await handler.Handle(new CreateOrderCommand(new CreateOrderDto(
            userId,
            [new CreateOrderItemDto(productId, 2)])), CancellationToken.None);

        result.TotalAmount.Should().Be(200m);
        result.Items.Should().HaveCount(1);
        (await db.Products.SingleAsync()).StockQuantity.Should().Be(3);
    }
}
