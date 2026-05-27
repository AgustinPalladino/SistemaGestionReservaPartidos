using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Team>> GetByTournamentAsync(int tournamentId, CancellationToken cancellationToken = default)
        => await _context.Teams
            .AsNoTracking()
            .Include(t => t.Captain)
            .Where(t => t.TournamentId == tournamentId)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public Task<Team?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Teams.AsNoTracking().Include(t => t.Captain).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Add(team);
        await _context.SaveChangesAsync(cancellationToken);
        return team;
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
