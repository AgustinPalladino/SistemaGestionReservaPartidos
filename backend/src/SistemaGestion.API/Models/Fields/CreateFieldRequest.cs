namespace SistemaGestion.API.Models.Fields;

public record CreateFieldRequest(
    string Name,
    string SportType,
    decimal PricePerHour);
