namespace Application.Interfaces.Persistence;

/// <summary>
/// Inicia un ámbito transaccional con bloqueo por cancha y fecha (implementación en Infrastructure).
/// </summary>
public interface IScheduleConflictCoordinator
{
    Task<IScheduleConflictScope> BeginFieldDateScopeAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
