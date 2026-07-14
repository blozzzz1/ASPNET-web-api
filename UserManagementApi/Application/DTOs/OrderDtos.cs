using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Application.DTOs;

public class OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}

public class OrderDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
}

public record CreateOrderItemDto(Guid ProductId, int Quantity);

public record CreateOrderDto(Guid UserId, IReadOnlyList<CreateOrderItemDto> Items);

public record UpdateOrderStatusDto(OrderStatus Status);
