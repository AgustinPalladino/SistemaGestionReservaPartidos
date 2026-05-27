namespace Application.Services.Matches;

/// <summary>
/// Validaciones de negocio para partidos (complemento al CHECK de base de datos).
/// </summary>
public static class MatchValidation
{
    public static void EnsureDifferentTeams(int homeTeamId, int awayTeamId)
    {
        if (homeTeamId == awayTeamId)
        {
            throw new InvalidOperationException(
                "El equipo local y el visitante deben ser distintos.");
        }
    }
}
