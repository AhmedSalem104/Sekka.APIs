using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Infrastructure.Repositories;
using Sekka.Persistence;

namespace Sekka.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly SekkaDbContext _context;
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    public UnitOfWork(SekkaDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
    {
        var typeName = typeof(TEntity).Name;
        return (IGenericRepository<TEntity, TKey>)_repositories.GetOrAdd(typeName,
            _ => new GenericRepository<TEntity, TKey>(_context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
