namespace SistemaGestion.API.Models.Reservations;

/// <summary>
/// Representa un bloque de hora en un día (disponible u ocupada)
/// </summary>
public class AvailableHourDto
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
