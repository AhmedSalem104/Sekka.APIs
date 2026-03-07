using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class NotificationChannelPreference : AuditableEntity<Guid>
{
    public Guid DriverId { get; set; }
    public NotificationType NotificationType { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;
    public string SoundName { get; set; } = "default";
    public bool VibrationEnabled { get; set; } = true;
    public string VibrationPattern { get; set; } = "default";
    public string LedColor { get; set; } = "#FFFFFF";
    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
    public bool ShowInLockScreen { get; set; } = true;
    public bool GroupAlerts { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
}
