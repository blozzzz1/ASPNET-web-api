using AutoMapper;
using FluentValidation;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Domain.Entities;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(CreateProductDto Product) : IRequest<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Product.Description).MaximumLength(2000);
        RuleFor(x => x.Product.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Product.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = mapper.Map<Product>(request.Product);
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;

        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}
