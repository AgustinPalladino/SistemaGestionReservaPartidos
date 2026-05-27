using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly AppDbContext _context;

    public MatchRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Match>> GetByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default)
        => await _context.Matches
            .AsNoTracking()
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Field)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);

    public Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Matches
            .AsNoTracking()
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Field)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Match> AddAsync(Match match, CancellationToken cancellationToken = default)
    {
        _context.Matches.Add(match);
        await _context.SaveChangesAsync(cancellationToken);
        return match;
    }

    public async Task UpdateAsync(Match match, CancellationToken cancellationToken = default)
    {
        _context.Matches.Update(match);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
