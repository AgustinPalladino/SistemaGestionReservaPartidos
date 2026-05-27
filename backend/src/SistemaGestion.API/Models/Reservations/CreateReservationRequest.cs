namespace SistemaGestion.API.Models.Reservations;

public record CreateReservationRequest(
    Guid UserId,
    int FieldId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal TotalAmount);
