namespace Application.Services.Reservations;

/// <summary>
/// Reglas puras de solapamiento de franjas horarias (sin dependencias de infraestructura).
/// </summary>
public static class ScheduleOverlapCalculator
{
    public static bool RangesOverlap(TimeOnly startA, TimeOnly endA, TimeOnly startB, TimeOnly endB)
        => startA < endB && startB < endA;

    public static bool MatchOverlaps(
        DateOnly targetDate,
        TimeOnly rangeStart,
        TimeOnly rangeEnd,
        DateTime matchScheduledAt,
        int matchDurationMinutes)
    {
        var matchDate = DateOnly.FromDateTime(matchScheduledAt);
        if (matchDate != targetDate)
        {
            return false;
        }

        var matchStart = TimeOnly.FromDateTime(matchScheduledAt);
        var matchEnd = matchStart.Add(TimeSpan.FromMinutes(matchDurationMinutes));
        return RangesOverlap(rangeStart, rangeEnd, matchStart, matchEnd);
    }
}