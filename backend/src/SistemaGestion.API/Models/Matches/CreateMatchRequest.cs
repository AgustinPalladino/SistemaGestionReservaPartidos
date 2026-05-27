using Domain.Enums;

namespace SistemaGestion.API.Models.Matches;

public record CreateMatchRequest(
    int TournamentId,
    int HomeTeamId,
    int AwayTeamId,
    int? FieldId,
    DateTime ScheduledAt,
    MatchStatus Status);
