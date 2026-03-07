using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Settings;

public class NotificationChannelPrefDto
{
    public NotificationType NotificationType { get; set; }
    public string NotificationTypeName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public bool SoundEnabled { get; set; }
    public string SoundName { get; set; } = "default";
    public bool VibrationEnabled { get; set; }
    public string VibrationPattern { get; set; } = "default";
    public string LedColor { get; set; } = "#FFFFFF";
    public NotificationPriority Priority { get; set; }
    public bool ShowInLockScreen { get; set; }
    public bool GroupAlerts { get; set; }
}

public class UpdateChannelPrefDto
{
    public NotificationType NotificationType { get; set; }
    public bool? IsEnabled { get; set; }
    public bool? SoundEnabled { get; set; }
    public string? SoundName { get; set; }
    public bool? VibrationEnabled { get; set; }
    public string? VibrationPattern { get; set; }
    public string? LedColor { get; set; }
    public NotificationPriority? Priority { get; set; }
    public bool? ShowInLockScreen { get; set; }
    public bool? GroupAlerts { get; set; }
}
