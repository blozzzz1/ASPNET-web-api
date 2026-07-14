using MediatR;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid Id) : IRequest;

public class DeleteOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository) : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Заказ с Id '{request.Id}' не найден.");

        if (order.Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new InvalidOperationException("Нельзя удалить отправленный или доставленный заказ.");

        foreach (var item in order.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is not null)
            {
                product.StockQuantity += item.Quantity;
                productRepository.Update(product);
            }
        }

        orderRepository.Remove(order);
        await orderRepository.SaveChangesAsync(cancellationToken);
    }
}
