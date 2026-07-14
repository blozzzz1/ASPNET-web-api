using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UserManagementApi.Application.DTOs;

namespace UserManagementApi.Tests.Integration;

public class ProductsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ProductsApiTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GetProducts_WithAuth_ReturnsSeedProducts()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().HaveCountGreaterThanOrEqualTo(2);
        products.Should().Contain(p => p.Name == "Ноутбук");
    }

    [Fact]
    public async Task CreateProduct_WithAuth_ReturnsCreated()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Клавиатура",
            description = "Механическая",
            price = 5490m,
            stockQuantity = 20
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product!.Name.Should().Be("Клавиатура");
        product.IsActive.Should().BeTrue();
    }
}
