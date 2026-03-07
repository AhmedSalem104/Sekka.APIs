using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Settings;

public class UpdateDriverPreferencesDto
{
    public ThemeMode? Theme { get; set; }
    public string? Language { get; set; }
    public int? NumberFormat { get; set; }
    public bool? FocusModeAutoTrigger { get; set; }
    public int? FocusModeSpeedThreshold { get; set; }
    public bool? TextToSpeechEnabled { get; set; }
    public bool? HapticFeedback { get; set; }
    public bool? HighContrastMode { get; set; }
    public MapApp? PreferredMapApp { get; set; }
    public int? MaxOrdersPerShift { get; set; }
    public bool? AutoSendReceipt { get; set; }
    public int? LocationTrackingInterval { get; set; }
    public int? OfflineSyncInterval { get; set; }
}

public class UpdateFocusModeDto
{
    public bool AutoTrigger { get; set; }
    public int SpeedThreshold { get; set; }
}

public class UpdateQuietHoursDto
{
    public bool Enabled { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}

public class UpdateNotificationPrefsDto
{
    public bool? NotifyNewOrder { get; set; }
    public bool? NotifyCashAlert { get; set; }
    public bool? NotifyBreakReminder { get; set; }
    public bool? NotifyMaintenance { get; set; }
    public bool? NotifySettlement { get; set; }
    public bool? NotifyAchievement { get; set; }
    public bool? NotifySound { get; set; }
    public bool? NotifyVibration { get; set; }
}

public class UpdateCostParamsDto
{
    public decimal? FuelPricePerLiter { get; set; }
    public decimal? FuelConsumptionPer100Km { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? DepreciationPerKm { get; set; }
}

public class UpdateHomeLocationDto
{
    public double HomeLatitude { get; set; }
    public double HomeLongitude { get; set; }
    public string? HomeAddress { get; set; }
}
