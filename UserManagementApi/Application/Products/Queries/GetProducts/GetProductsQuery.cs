using AutoMapper;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;

public class GetProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<ProductDto>>(products);
    }
}
