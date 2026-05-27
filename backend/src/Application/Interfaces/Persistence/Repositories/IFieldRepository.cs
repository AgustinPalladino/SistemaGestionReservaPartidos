using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IFieldRepository
{
    Task<IReadOnlyList<Field>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken = default);
    Task<Field?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Field> AddAsync(Field field, CancellationToken cancellationToken = default);
    Task UpdateAsync(Field field, CancellationToken cancellationToken = default);
}
