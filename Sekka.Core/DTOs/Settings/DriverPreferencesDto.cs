using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Settings;

public class DriverPreferencesDto
{
    public ThemeMode Theme { get; set; }
    public string Language { get; set; } = "ar";
    public int NumberFormat { get; set; }
    public bool FocusModeAutoTrigger { get; set; }
    public int FocusModeSpeedThreshold { get; set; }
    public bool TextToSpeechEnabled { get; set; }
    public bool HapticFeedback { get; set; }
    public bool HighContrastMode { get; set; }
    public bool NotifyNewOrder { get; set; }
    public bool NotifyCashAlert { get; set; }
    public bool NotifyBreakReminder { get; set; }
    public bool NotifyMaintenance { get; set; }
    public bool NotifySettlement { get; set; }
    public bool NotifyAchievement { get; set; }
    public bool NotifySound { get; set; }
    public bool NotifyVibration { get; set; }
    public TimeOnly? QuietHoursStart { get; set; }
    public TimeOnly? QuietHoursEnd { get; set; }
    public MapApp PreferredMapApp { get; set; }
    public int? MaxOrdersPerShift { get; set; }
    public bool AutoSendReceipt { get; set; }
    public int LocationTrackingInterval { get; set; }
    public int OfflineSyncInterval { get; set; }
    public double? HomeLatitude { get; set; }
    public double? HomeLongitude { get; set; }
    public string? HomeAddress { get; set; }
    public bool BackToBaseAlertEnabled { get; set; }
    public decimal BackToBaseRadiusKm { get; set; }
}
