using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReservationScheduleReadRepository : IReservationScheduleReadRepository
{
    private readonly AppDbContext _context;

    public ReservationScheduleReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsOverlappingReservationAsync(
        int fieldId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        Guid? excludeReservationId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reservations
            .AsNoTracking()
            .AnyAsync(
                r => r.FieldId == fieldId
                     && r.Date == date
                     && r.Status != ReservationStatus.Cancelled
                     && (excludeReservationId == null || r.Id != excludeReservationId.Value)
                     && r.StartTime < end
                     && start < r.EndTime,
                cancellationToken);
    }

    public async Task<IReadOnlyList<DateTime>> GetBlockingMatchScheduledTimesAsync(
        int fieldId,
        CancellationToken cancellationToken = default)
    {
        var blockingStatuses = new[] { MatchStatus.Scheduled, MatchStatus.InProgress };

        return await _context.Matches
            .AsNoTracking()
            .Where(m => m.FieldId == fieldId && blockingStatuses.Contains(m.Status))
            .Select(m => m.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetReservationsByMonthAsync(
        int fieldId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.FieldId == fieldId
                     && r.Date.Year == year
                     && r.Date.Month == month
                     && r.Status != ReservationStatus.Cancelled)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetReservationsByDateAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.FieldId == fieldId
                     && r.Date == date
                     && r.Status != ReservationStatus.Cancelled)
            .OrderBy(r => r.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DateTime>> GetMatchesByDateAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var blockingStatuses = new[] { MatchStatus.Scheduled, MatchStatus.InProgress };

        return await _context.Matches
            .AsNoTracking()
            .Where(m => m.FieldId == fieldId
                     && m.ScheduledAt.Date == date.ToDateTime(TimeOnly.MinValue)
                     && blockingStatuses.Contains(m.Status))
            .Select(m => m.ScheduledAt)
            .OrderBy(m => m)
            .ToListAsync(cancellationToken);
    }
}
