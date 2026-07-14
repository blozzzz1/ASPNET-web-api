using MediatR;
using UserManagementApi.Infrastructure.Repositories;

namespace UserManagementApi.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;

public class DeleteProductCommandHandler(IProductRepository productRepository) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Товар с Id '{request.Id}' не найден.");

        productRepository.Remove(product);
        await productRepository.SaveChangesAsync(cancellationToken);
    }
}
