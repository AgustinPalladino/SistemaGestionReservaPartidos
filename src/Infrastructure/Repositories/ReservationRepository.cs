using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Field)
            .OrderByDescending(r => r.Date)
            .ThenBy(r => r.StartTime)
            .ToListAsync(cancellationToken);
    }

    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Field)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
