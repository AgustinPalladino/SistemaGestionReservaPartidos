using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Reserva de una cancha por un usuario en una fecha y franja horaria.
/// </summary>
public class Reservation
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int FieldId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public ReservationStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;

    public Field Field { get; set; } = null!;
}
