using Domain.Enums;

namespace SistemaGestion.API.Models.Tournaments;

public record CreateTournamentRequest(
    Guid OrganizerId,
    string Name,
    string SportType,
    TournamentStatus Status,
    DateOnly StartDate,
    DateOnly EndDate);
