namespace Sekka.Persistence.Entities.Base;

public abstract class AuditableEntity<TKey> : BaseEntity<TKey>
{
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
