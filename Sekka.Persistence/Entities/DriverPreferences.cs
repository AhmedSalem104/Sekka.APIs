using Sekka.Core.Enums;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class DriverPreferences : AuditableEntity<Guid>
{
    public Guid DriverId { get; set; }
    public ThemeMode Theme { get; set; }
    public string Language { get; set; } = "ar";
    public int NumberFormat { get; set; }
    public bool FocusModeAutoTrigger { get; set; } = true;
    public int FocusModeSpeedThreshold { get; set; } = 20;
    public bool TextToSpeechEnabled { get; set; }
    public decimal TextToSpeechSpeed { get; set; } = 1.0m;
    public bool HapticFeedback { get; set; } = true;
    public bool HighContrastMode { get; set; }
    public bool NotifyNewOrder { get; set; } = true;
    public bool NotifyCashAlert { get; set; } = true;
    public bool NotifyBreakReminder { get; set; } = true;
    public bool NotifyMaintenance { get; set; } = true;
    public bool NotifySettlement { get; set; } = true;
    public bool NotifyAchievement { get; set; } = true;
    public bool NotifySound { get; set; } = true;
    public bool NotifyVibration { get; set; } = true;
    public TimeOnly? QuietHoursStart { get; set; }
    public TimeOnly? QuietHoursEnd { get; set; }
    public MapApp PreferredMapApp { get; set; }
    public int? MaxOrdersPerShift { get; set; }
    public bool AutoSendReceipt { get; set; } = true;
    public int LocationTrackingInterval { get; set; } = 10;
    public int OfflineSyncInterval { get; set; } = 30;
    public double? HomeLatitude { get; set; }
    public double? HomeLongitude { get; set; }
    public string? HomeAddress { get; set; }
    public bool BackToBaseAlertEnabled { get; set; }
    public decimal BackToBaseRadiusKm { get; set; } = 2.0m;

    // Navigation
    public Driver Driver { get; set; } = null!;
}
