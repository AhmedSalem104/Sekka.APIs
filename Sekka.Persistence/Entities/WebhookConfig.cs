using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class WebhookConfig : BaseEntity<Guid>
{
    public Guid? DriverId { get; set; }
    public Guid? PartnerId { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public string Events { get; set; } = null!; // JSON
    public bool IsActive { get; set; } = true;
    public DateTime? LastTriggeredAt { get; set; }
    public int FailureCount { get; set; }

    // Navigation
    public Driver? Driver { get; set; }
    public Partner? Partner { get; set; }
}
