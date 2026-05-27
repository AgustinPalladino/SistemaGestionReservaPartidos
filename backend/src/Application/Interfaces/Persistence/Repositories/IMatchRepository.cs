using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IMatchRepository
{
    Task<IReadOnlyList<Match>> GetByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default);
    Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Match> AddAsync(Match match, CancellationToken cancellationToken = default);
    Task UpdateAsync(Match match, CancellationToken cancellationToken = default);
}
