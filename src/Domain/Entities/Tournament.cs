using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Torneo organizado por un usuario con rol de organizador.
/// </summary>
public class Tournament
{
    public int Id { get; set; }

    public Guid OrganizerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SportType { get; set; } = string.Empty;

    public TournamentStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public User Organizer { get; set; } = null!;

    public ICollection<Team> Teams { get; set; } = new List<Team>();

    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
