using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class SystemNotice : BaseEntity<Guid>
{
    public string Title { get; set; } = null!;
    public string? TitleEn { get; set; }
    public string Body { get; set; } = null!;
    public string? BodyEn { get; set; }
    public SystemNoticeType NoticeType { get; set; }
    public TargetAudience TargetAudience { get; set; }
    public Guid? TargetRegionId { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionLabel { get; set; }
    public string? BackgroundColor { get; set; }
    public string? IconUrl { get; set; }
    public int Priority { get; set; } = 1;
    public DateTime StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsDismissable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int ViewCount { get; set; }
    public int ClickCount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Region? TargetRegion { get; set; }
}
