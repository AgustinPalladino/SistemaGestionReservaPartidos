namespace Application.Models.Reservations;

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Type { get; set; } = "Reservation";
    public string? Title { get; set; }
}
