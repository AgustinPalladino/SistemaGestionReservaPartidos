using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface ITournamentRepository
{
    Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tournament?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tournament> AddAsync(Tournament tournament, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tournament tournament, CancellationToken cancellationToken = default);
}
