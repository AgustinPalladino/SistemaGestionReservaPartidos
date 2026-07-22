using Application.Models.Reservations;

namespace Application.Interfaces;

/// <summary>
/// Valida solapamientos de horario en canchas entre reservas y partidos.
/// </summary>
public interface IReservationScheduleService
{
    /// <summary>
    /// Indica si existe conflicto de horario en la cancha para la fecha y franja indicadas.
    /// Considera reservas activas (no canceladas) y partidos programados o en curso.
    /// </summary>
    Task<bool> HasScheduleConflictAsync(
        int fieldId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Igual que <see cref="HasScheduleConflictAsync"/> excluyendo una reserva existente (p. ej. al editar).
    /// </summary>
    Task<bool> HasScheduleConflictAsync(
        int fieldId,
        DateOnly date,
        TimeOnly start,
        TimeOnly end,
        Guid? excludeReservationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CalendarEventDto>> GetCalendarEventsAsync(
        int fieldId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AvailableHourDto>> GetAvailableHoursAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
