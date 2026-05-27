namespace Application.Interfaces.Persistence;

/// <summary>
/// Ámbito transaccional con bloqueo pesimista para validar conflictos de horario en una cancha/fecha.
/// </summary>
public interface IScheduleConflictScope : IAsyncDisposable
{
}
