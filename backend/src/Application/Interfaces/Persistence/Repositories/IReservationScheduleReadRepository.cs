namespace Application.Interfaces.Persistence.Repositories;

/// <summary>
/// Lecturas de datos necesarias para detectar solapamientos de horario en canchas.
/// </summary>
public interface IReservationScheduleReadRepository
{
    Task<bool> ExistsOverlappingReservationAsync(
        int fieldId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        Guid? excludeReservationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DateTime>> GetBlockingMatchScheduledTimesAsync(
        int fieldId,
        CancellationToken cancellationToken = default);
}
