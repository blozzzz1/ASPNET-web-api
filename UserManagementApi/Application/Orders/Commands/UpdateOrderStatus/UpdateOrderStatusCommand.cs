using AutoMapper;
using FluentValidation;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid Id, UpdateOrderStatusDto Status) : IRequest<OrderDto>;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status.Status).IsInEnum();
    }
}

public class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IMapper mapper) : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Заказ с Id '{request.Id}' не найден.");

        if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
            throw new InvalidOperationException(
                $"Нельзя изменить статус заказа в состоянии '{order.Status}'.");

        order.Status = request.Status.Status;
        order.UpdatedAt = DateTime.UtcNow;

        orderRepository.Update(order);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<OrderDto>(order);
    }
}
