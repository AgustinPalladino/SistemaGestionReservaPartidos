using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Partido de torneo entre dos equipos, opcionalmente asignado a una cancha.
/// <see cref="HomeTeamId"/> y <see cref="AwayTeamId"/> deben ser equipos distintos
/// (restricción CHECK en base de datos; validar también en capa de servicio al crear/actualizar).
/// </summary>
public class Match
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int HomeTeamId { get; set; }

    public int AwayTeamId { get; set; }

    public int? FieldId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public int? HomeScore { get; set; }

    public int? AwayScore { get; set; }

    public MatchStatus Status { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public Team HomeTeam { get; set; } = null!;

    public Team AwayTeam { get; set; } = null!;

    public Field? Field { get; set; }
}
