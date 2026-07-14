using UserManagementApi.Domain.Entities;

namespace UserManagementApi.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
    void Remove(Order order);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
