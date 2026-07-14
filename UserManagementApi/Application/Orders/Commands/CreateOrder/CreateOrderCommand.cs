using AutoMapper;
using FluentValidation;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto Order) : IRequest<OrderDto>;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Order.UserId).NotEmpty();
        RuleFor(x => x.Order.Items).NotEmpty();
        RuleForEach(x => x.Order.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Order.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Пользователь с Id '{request.Order.UserId}' не найден.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var itemDto in request.Order.Items)
        {
            var product = await productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Товар с Id '{itemDto.ProductId}' не найден.");

            if (!product.IsActive)
                throw new InvalidOperationException($"Товар '{product.Name}' недоступен.");

            if (product.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException(
                    $"Недостаточно товара '{product.Name}'. В наличии: {product.StockQuantity}.");

            product.StockQuantity -= itemDto.Quantity;
            productRepository.Update(product);

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        order.User = user;
        foreach (var item in order.Items)
        {
            item.Product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                ?? throw new InvalidOperationException($"Товар с Id '{item.ProductId}' не найден после создания заказа.");
        }

        return mapper.Map<OrderDto>(order);
    }
}
