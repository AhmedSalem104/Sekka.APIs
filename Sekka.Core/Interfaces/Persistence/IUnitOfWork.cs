namespace Sekka.Core.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
