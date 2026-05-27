using Application.Interfaces.Persistence.Repositories;
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
}
