namespace Application.Services.Reservations;

/// <summary>
/// Parámetros para la detección de solapamiento con partidos de torneo.
/// Los partidos no tienen hora de fin en el modelo; se usa una duración por defecto.
/// </summary>
public static class ScheduleConflictOptions
{
    /// <summary>
    /// Duración asumida de un partido al evaluar conflictos de cancha (minutos).
    /// </summary>
    public const int DefaultMatchDurationMinutes = 90;
}
