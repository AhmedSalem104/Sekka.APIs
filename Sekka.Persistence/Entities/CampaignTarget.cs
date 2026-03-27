using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class CampaignTarget : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;
    public CampaignType CampaignType { get; set; }
    public Guid? SegmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public string MessageTemplate { get; set; } = null!;
    public NotificationChannel Channel { get; set; }
    public int TargetCount { get; set; }
    public int SentCount { get; set; }
    public int OpenCount { get; set; }
    public int ConversionCount { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public CampaignStatus Status { get; set; }
    public new Guid CreatedBy { get; set; }

    // Navigation
    public CustomerSegment? Segment { get; set; }
    public InterestCategory? Category { get; set; }
}
