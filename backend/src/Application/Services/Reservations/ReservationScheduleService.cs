using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Models.Reservations;

namespace Application.Services.Reservations;

/// <summary>
/// Lógica de negocio para detectar conflictos de horario entre reservas y partidos.
/// </summary>
public class ReservationScheduleService : IReservationScheduleService
{
    private readonly IScheduleConflictCoordinator _coordinator;
    private readonly IReservationScheduleReadRepository _readRepository;

    private static readonly TimeOnly DayStart = new(6, 0);
    private static readonly TimeOnly DayEnd = new(22, 0);
    private static readonly int SlotDurationMinutes = 60;

    public ReservationScheduleService(
        IScheduleConflictCoordinator coordinator,
        IReservationScheduleReadRepository readRepository)
    {
        _coordinator = coordinator;
        _readRepository = readRepository;
    }

    public Task<bool> HasScheduleConflictAsync(
        int fieldId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        CancellationToken cancellationToken = default)
        => HasScheduleConflictAsync(fieldId, date, start, end, excludeReservationId: null, cancellationToken);

    public async Task<bool> HasScheduleConflictAsync(
        int fieldId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        Guid? excludeReservationId,
        CancellationToken cancellationToken = default)
    {
        if (end <= start)
        {
            throw new ArgumentException("La hora de fin debe ser posterior a la de inicio.", nameof(end));
        }

        await using var scope = await _coordinator.BeginFieldDateScopeAsync(fieldId, date, cancellationToken);

        var reservationConflict = await _readRepository.ExistsOverlappingReservationAsync(
            fieldId, date, start, end, excludeReservationId, cancellationToken);

        if (reservationConflict)
        {
            return true;
        }

        var matchTimes = await _readRepository.GetBlockingMatchScheduledTimesAsync(fieldId, cancellationToken);

        return matchTimes.Any(scheduledAt =>
            ScheduleOverlapCalculator.MatchOverlaps(
                date,
                start,
                end,
                scheduledAt,
                ScheduleConflictOptions.DefaultMatchDurationMinutes));
    }

    public async Task<IReadOnlyList<CalendarEventDto>> GetCalendarEventsAsync(
        int fieldId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var reservations = await _readRepository.GetReservationsByMonthAsync(fieldId, year, month, cancellationToken);
        var events = reservations.Select(r => new CalendarEventDto
        {
            Id = r.Id,
            Date = r.Date,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Type = "Reservation",
            Title = $"Reserva {r.StartTime:HH:mm}"
        }).ToList();

        return events;
    }

    public async Task<IReadOnlyList<AvailableHourDto>> GetAvailableHoursAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var reservations = await _readRepository.GetReservationsByDateAsync(fieldId, date, cancellationToken);
        var matches = await _readRepository.GetMatchesByDateAsync(fieldId, date, cancellationToken);

        var occupied = new List<(TimeOnly Start, TimeOnly End)>();
        occupied.AddRange(reservations.Select(r => (r.StartTime, r.EndTime)));
        occupied.AddRange(matches.Select(m =>
        {
            var scheduledDate = DateOnly.FromDateTime(m.Date);
            var scheduledTime = TimeOnly.FromDateTime(m);
            return (scheduledTime, scheduledTime.AddHours(1));
        }));

        var slots = new List<AvailableHourDto>();
        var current = DayStart;

        while (current < DayEnd)
        {
            var next = current.AddMinutes(SlotDurationMinutes);
            var isAvailable = !occupied.Any(o =>
                current < o.End && o.Start < next);

            slots.Add(new AvailableHourDto
            {
                StartTime = current,
                EndTime = next,
                IsAvailable = isAvailable
            });

            current = next;
        }

        return slots;
    }
}
