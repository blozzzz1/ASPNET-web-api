using AutoMapper;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public class GetOrdersQueryHandler(
    IOrderRepository orderRepository,
    IMapper mapper) : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<OrderDto>>(orders);
    }
}
