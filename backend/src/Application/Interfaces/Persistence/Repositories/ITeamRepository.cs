using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface ITeamRepository
{
    Task<IReadOnlyList<Team>> GetByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
}
