using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is BaseEntity<Guid> baseEntity)
                {
                    if (baseEntity.Id == Guid.Empty)
                        baseEntity.Id = Guid.NewGuid();
                    baseEntity.CreatedAt = DateTime.UtcNow;
                }

                if (entry.Entity is AuditableEntity<Guid> auditable)
                {
                    auditable.CreatedBy ??= "System";
                }
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is AuditableEntity<Guid> auditable)
                {
                    auditable.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
