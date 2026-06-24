namespace Application.Interfaces.Persistence.Repositories;
using Domain.Entities;

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

    Task<IReadOnlyList<Reservation>> GetReservationsByMonthAsync(
        int fieldId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Reservation>> GetReservationsByDateAsync(
        int fieldId,
        DateOnly date,

        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DateTime>> GetMatchesByDateAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
