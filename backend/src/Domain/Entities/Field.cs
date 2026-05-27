namespace Domain.Entities;

/// <summary>
/// Cancha deportiva disponible para reservas y partidos de torneo.
/// <see cref="IsActive"/> permite desactivar la cancha sin borrado físico.
/// </summary>
public class Field
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SportType { get; set; } = string.Empty;

    public decimal PricePerHour { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
