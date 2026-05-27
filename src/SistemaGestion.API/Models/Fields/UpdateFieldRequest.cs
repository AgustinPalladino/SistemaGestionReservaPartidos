namespace SistemaGestion.API.Models.Fields;

public record UpdateFieldRequest(
    string Name,
    string SportType,
    decimal PricePerHour,
    bool IsActive);
