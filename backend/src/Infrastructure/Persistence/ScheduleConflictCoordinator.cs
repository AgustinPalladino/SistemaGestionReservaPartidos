using System.Data;
using Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class ScheduleConflictCoordinator : IScheduleConflictCoordinator
{
    private readonly AppDbContext _context;

    public ScheduleConflictCoordinator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IScheduleConflictScope> BeginFieldDateScopeAsync(
        int fieldId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var lockKey = (long)fieldId * 1_000_000L + date.DayNumber;
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_xact_lock({0})",
            [lockKey],
            cancellationToken);

        return new ScheduleConflictScope(transaction);
    }

    private sealed class ScheduleConflictScope : IScheduleConflictScope
    {
        private readonly IDbContextTransaction _transaction;

        public ScheduleConflictScope(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
        }
    }
}
