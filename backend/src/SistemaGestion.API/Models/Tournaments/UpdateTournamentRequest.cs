using Domain.Enums;

namespace SistemaGestion.API.Models.Tournaments;

public record UpdateTournamentRequest(
    string Name,
    string SportType,
    TournamentStatus Status,
    DateOnly StartDate,
    DateOnly EndDate);
