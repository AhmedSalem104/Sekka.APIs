using Microsoft.EntityFrameworkCore;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Specifications;
using Sekka.Persistence;

namespace Sekka.Infrastructure.Repositories;

public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : class
{
    private readonly SekkaDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(SekkaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec)
    {
        return await SpecificationEvaluator<TEntity>.GetQuery(_dbSet.AsQueryable(), spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec)
    {
        var query = _dbSet.AsQueryable();
        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);
        return await query.CountAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}
