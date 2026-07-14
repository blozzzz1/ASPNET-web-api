using AutoMapper;
using FluentValidation;
using MediatR;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, UpdateProductDto Product) : IRequest<ProductDto>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Product.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Product.Description).MaximumLength(2000);
        RuleFor(x => x.Product.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Product.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Товар с Id '{request.Id}' не найден.");

        mapper.Map(request.Product, product);
        productRepository.Update(product);
        await productRepository.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}
