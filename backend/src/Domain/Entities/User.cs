using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Usuario del sistema con credenciales y rol de acceso.
/// La contraseña se almacena únicamente como hash; nunca en texto plano.
/// </summary>
public class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public ICollection<Tournament> OrganizedTournaments { get; set; } = new List<Tournament>();

    public ICollection<Team> CaptainedTeams { get; set; } = new List<Team>();
}
