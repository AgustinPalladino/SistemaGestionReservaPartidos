using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FieldRepository : IFieldRepository
{
    private readonly AppDbContext _context;

    public FieldRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Field>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _context.Fields.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(f => f.IsActive);
        }

        return await query.OrderBy(f => f.Name).ToListAsync(cancellationToken);
    }

    public Task<Field?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _context.Fields.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<Field> AddAsync(Field field, CancellationToken cancellationToken = default)
    {
        _context.Fields.Add(field);
        await _context.SaveChangesAsync(cancellationToken);
        return field;
    }

    public async Task UpdateAsync(Field field, CancellationToken cancellationToken = default)
    {
        _context.Fields.Update(field);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
