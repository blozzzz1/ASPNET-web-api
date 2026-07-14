using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Remove(Product product);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
