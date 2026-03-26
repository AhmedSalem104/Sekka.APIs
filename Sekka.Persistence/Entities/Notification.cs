using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Notification : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
    public string? ActionType { get; set; }
    public string? ActionData { get; set; }
    public NotificationPriority Priority { get; set; }
    public DateTime? ReadAt { get; set; }

    public Driver Driver { get; set; } = null!;
}
