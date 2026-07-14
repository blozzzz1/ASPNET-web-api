using AutoMapper;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Товар с Id '{request.Id}' не найден.");

        return mapper.Map<ProductDto>(product);
    }
}
