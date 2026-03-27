using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class WebhookLog : BaseEntity<Guid>
{
    public Guid WebhookConfigId { get; set; }
    public string EventType { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public int? ResponseStatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public bool IsSuccess { get; set; }
    public int RetryCount { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public WebhookConfig WebhookConfig { get; set; } = null!;
}
