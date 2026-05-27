using Domain.Enums;

namespace SistemaGestion.API.Models.Matches;

public record UpdateMatchScoreRequest(
    int HomeScore,
    int AwayScore,
    MatchStatus Status);
