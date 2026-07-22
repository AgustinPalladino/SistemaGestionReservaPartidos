namespace SistemaGestion.API.Models.Reservations;
using Domain.Enums;

/// <summary>
/// Evento de calendario: puede ser una Reserva o un Match
/// </summary>
public class CalendarEventDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public EventType Type { get; set; } // "Reservation" o "Match"
    public string? Title { get; set; }
}
