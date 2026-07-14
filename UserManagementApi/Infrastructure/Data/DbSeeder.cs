using Microsoft.AspNetCore.Identity;
using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        if (context.Users.Any())
            return;

        var user = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "Иван",
            LastName = "Петров",
            Email = "ivan.petrov@example.com",
            Phone = "+79001234567",
            Role = "Admin"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        var products = new[]
        {
            new Product
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                Name = "Ноутбук",
                Description = "15\" ноутбук",
                Price = 89990m,
                StockQuantity = 10
            },
            new Product
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                Name = "Мышь",
                Description = "Беспроводная мышь",
                Price = 1990m,
                StockQuantity = 50
            }
        };

        context.Users.Add(user);
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
