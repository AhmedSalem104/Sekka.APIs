using System.ComponentModel.DataAnnotations;
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
    [Range(0.01, 1000, ErrorMessage = "سعر الوقود لازم يكون بين 0.01 و 1000")]
    public decimal? FuelPricePerLiter { get; set; }

    [Range(0.01, 100, ErrorMessage = "استهلاك الوقود لازم يكون بين 0.01 و 100")]
    public decimal? FuelConsumptionPer100Km { get; set; }

    [Range(0.01, 10000, ErrorMessage = "سعر الساعة لازم يكون بين 0.01 و 10000")]
    public decimal? HourlyRate { get; set; }

    [Range(0.01, 100, ErrorMessage = "تكلفة الإهلاك لازم تكون بين 0.01 و 100")]
    public decimal? DepreciationPerKm { get; set; }
}

public class UpdateHomeLocationDto
{
    public double HomeLatitude { get; set; }
    public double HomeLongitude { get; set; }
    public string? HomeAddress { get; set; }
}
