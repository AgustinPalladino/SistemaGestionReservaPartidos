namespace Application.Models.Reservations;

public class AvailableHourDto
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
