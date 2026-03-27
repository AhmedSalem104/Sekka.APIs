using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CustomerSegment : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public SegmentType SegmentType { get; set; }
    public string? Description { get; set; }
    public string? ColorHex { get; set; }
    public string? Rules { get; set; }
    public bool IsAutomatic { get; set; } = true;
    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }
    public bool IsActive { get; set; } = true;
    public int MemberCount { get; set; }
    public DateTime? LastRefreshedAt { get; set; }

    // Navigation
    public ICollection<CustomerSegmentMember> Members { get; set; } = new List<CustomerSegmentMember>();
}
