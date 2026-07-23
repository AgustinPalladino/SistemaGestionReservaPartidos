using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveReservationForUserAsync(
        Guid userId,
        int fieldId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default);
    Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
