namespace Sekka.Persistence.Entities.Base;

public abstract class SoftDeletableEntity<TKey> : AuditableEntity<TKey>
{
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
