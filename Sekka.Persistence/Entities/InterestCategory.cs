using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class InterestCategory : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string? IconUrl { get; set; }
    public string? ColorHex { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public InterestCategory? ParentCategory { get; set; }
    public ICollection<InterestCategory> Children { get; set; } = new List<InterestCategory>();
    public ICollection<CustomerInterest> CustomerInterests { get; set; } = new List<CustomerInterest>();
}
