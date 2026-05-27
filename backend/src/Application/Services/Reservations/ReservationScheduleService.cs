using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;

namespace Application.Services.Reservations;

/// <summary>
/// Lógica de negocio para detectar conflictos de horario entre reservas y partidos.
/// </summary>
public class ReservationScheduleService : IReservationScheduleService
{
    private readonly IScheduleConflictCoordinator _coordinator;
    private readonly IReservationScheduleReadRepository _readRepository;

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
}
