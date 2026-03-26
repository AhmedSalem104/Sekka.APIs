using Sekka.Core.Enums;

namespace Sekka.Persistence.Entities;

public class NotificationLog
{
    public long Id { get; set; }
    public Guid RecipientId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string? TemplateName { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExternalId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
