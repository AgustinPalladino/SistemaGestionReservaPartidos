using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
}
