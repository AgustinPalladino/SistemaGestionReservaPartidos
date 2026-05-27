using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly AppDbContext _context;

    public TournamentRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Tournaments
            .AsNoTracking()
            .Include(t => t.Organizer)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);

    public Task<Tournament?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Tournaments
            .AsNoTracking()
            .Include(t => t.Organizer)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<Tournament> AddAsync(Tournament tournament, CancellationToken cancellationToken = default)
    {
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync(cancellationToken);
        return tournament;
    }

    public async Task UpdateAsync(Tournament tournament, CancellationToken cancellationToken = default)
    {
        _context.Tournaments.Update(tournament);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
