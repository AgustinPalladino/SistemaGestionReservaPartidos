namespace Domain.Entities;

/// <summary>
/// Equipo inscrito en un torneo con estadísticas de puntos y partidos jugados.
/// </summary>
public class Team
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid? CaptainId { get; set; }

    public int Points { get; set; }

    public int MatchesPlayed { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public User? Captain { get; set; }

    public ICollection<Match> HomeMatches { get; set; } = new List<Match>();

    public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
}
