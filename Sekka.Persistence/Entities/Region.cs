using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Region : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string? NameEn { get; set; }
    public Guid? ParentRegionId { get; set; }
    public double? CenterLatitude { get; set; }
    public double? CenterLongitude { get; set; }
    public double? RadiusKm { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Region? ParentRegion { get; set; }
    public ICollection<Region> Children { get; set; } = new List<Region>();
}
