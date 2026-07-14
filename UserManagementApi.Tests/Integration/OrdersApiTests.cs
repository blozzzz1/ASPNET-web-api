using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Tests.Integration;

public class OrdersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public OrdersApiTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateOrder_WithAuth_CreatesOrderAndReducesStock()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var productsBefore = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        var mouse = productsBefore!.Single(p => p.Name == "Мышь");
        var stockBefore = mouse.StockQuantity;

        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            userId = "11111111-1111-1111-1111-111111111111",
            items = new[]
            {
                new { productId = mouse.Id, quantity = 2 }
            }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(mouse.Price * 2);
        order.Status.Should().Be(OrderStatus.Pending);

        var productsAfter = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        productsAfter!.Single(p => p.Id == mouse.Id).StockQuantity.Should().Be(stockBefore - 2);
    }

    [Fact]
    public async Task UpdateOrderStatus_WithAuth_UpdatesStatus()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        var notebook = products!.Single(p => p.Name == "Ноутбук");

        var createResponse = await client.PostAsJsonAsync("/api/orders", new
        {
            userId = "11111111-1111-1111-1111-111111111111",
            items = new[] { new { productId = notebook.Id, quantity = 1 } }
        });
        var order = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        var patchResponse = await client.PatchAsJsonAsync($"/api/orders/{order!.Id}/status", new
        {
            status = OrderStatus.Confirmed
        });

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<OrderDto>();
        updated!.Status.Should().Be(OrderStatus.Confirmed);
    }
}
