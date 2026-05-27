namespace SistemaGestion.API.Models.Teams;

public record CreateTeamRequest(
    int TournamentId,
    string Name,
    Guid? CaptainId);
