using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.System;

// ══════════════════════════════════════
// App Config
// ══════════════════════════════════════
public class AppConfigDto
{
    public string ConfigKey { get; set; } = null!;
    public string ConfigValue { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateConfigDto
{
    public string ConfigValue { get; set; } = null!;
}

// ══════════════════════════════════════
// App Version
// ══════════════════════════════════════
public class AppVersionDto
{
    public Guid Id { get; set; }
    public AppPlatform Platform { get; set; }
    public string VersionCode { get; set; } = null!;
    public string VersionNumber { get; set; } = null!;
    public string MinRequiredVersion { get; set; } = null!;
    public int MinRequiredVersionNumber { get; set; }
    public string StoreUrl { get; set; } = null!;
    public string? ReleaseNotes { get; set; }
    public bool IsForceUpdate { get; set; }
    public bool IsActive { get; set; }
    public DateTime ReleasedAt { get; set; }
}

public class CreateAppVersionDto
{
    public AppPlatform Platform { get; set; }
    public string VersionCode { get; set; } = null!;
    public string VersionNumber { get; set; } = null!;
    public string MinRequiredVersion { get; set; } = null!;
    public int MinRequiredVersionNumber { get; set; }
    public string StoreUrl { get; set; } = null!;
    public string? ReleaseNotes { get; set; }
    public string? ReleaseNotesEn { get; set; }
    public bool IsForceUpdate { get; set; }
}

public class UpdateAppVersionDto
{
    public string? VersionCode { get; set; }
    public string? VersionNumber { get; set; }
    public string? MinRequiredVersion { get; set; }
    public int? MinRequiredVersionNumber { get; set; }
    public string? StoreUrl { get; set; }
    public string? ReleaseNotes { get; set; }
    public string? ReleaseNotesEn { get; set; }
    public bool? IsForceUpdate { get; set; }
    public bool? IsActive { get; set; }
}

public class AppVersionCheckDto
{
    public string CurrentVersion { get; set; } = null!;
    public string LatestVersion { get; set; } = null!;
    public string MinRequiredVersion { get; set; } = null!;
    public bool IsForceUpdate { get; set; }
    public string StoreUrl { get; set; } = null!;
    public string? ReleaseNotes { get; set; }
}

// ══════════════════════════════════════
// Feature Flags
// ══════════════════════════════════════
public class FeatureFlagDto
{
    public Guid Id { get; set; }
    public string FeatureKey { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? DisplayNameEn { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public bool EnabledForPremiumOnly { get; set; }
    public int EnabledForPercentage { get; set; }
    public string? Category { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class CreateFeatureFlagDto
{
    public string FeatureKey { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? DisplayNameEn { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public bool EnabledForPremiumOnly { get; set; }
    public int EnabledForPercentage { get; set; } = 100;
    public string? MinAppVersion { get; set; }
    public string? Category { get; set; }
}

public class UpdateFeatureFlagDto
{
    public string? FeatureKey { get; set; }
    public string? DisplayName { get; set; }
    public string? DisplayNameEn { get; set; }
    public string? Description { get; set; }
    public bool? IsEnabled { get; set; }
    public bool? EnabledForPremiumOnly { get; set; }
    public int? EnabledForPercentage { get; set; }
    public string? MinAppVersion { get; set; }
    public string? Category { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class FeatureFlagsCheckDto
{
    public Dictionary<string, bool> Features { get; set; } = new();
}

// ══════════════════════════════════════
// Maintenance Window
// ══════════════════════════════════════
public class MaintenanceWindowDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? TitleEn { get; set; }
    public string Message { get; set; } = null!;
    public string? MessageEn { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsFullBlock { get; set; }
}

public class CreateMaintenanceWindowDto
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsFullBlock { get; set; }
    public List<string>? AffectedServices { get; set; }
}

public class UpdateMaintenanceWindowDto
{
    public string? Title { get; set; }
    public string? TitleEn { get; set; }
    public string? Message { get; set; }
    public string? MessageEn { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFullBlock { get; set; }
    public List<string>? AffectedServices { get; set; }
}

public class InstantMaintenanceDto
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public bool IsFullBlock { get; set; }
}

// ══════════════════════════════════════
// System Notice
// ══════════════════════════════════════
public class SystemNoticeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? TitleEn { get; set; }
    public string Body { get; set; } = null!;
    public string? BodyEn { get; set; }
    public SystemNoticeType NoticeType { get; set; }
    public TargetAudience TargetAudience { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionLabel { get; set; }
    public string? BackgroundColor { get; set; }
    public int Priority { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsDismissable { get; set; }
    public bool IsActive { get; set; }
    public int ViewCount { get; set; }
    public int ClickCount { get; set; }
}

// ══════════════════════════════════════
// Commission Settings
// ══════════════════════════════════════
public class CommissionSettingsDto
{
    public decimal DefaultCommissionPercent { get; set; }
    public decimal MinCommission { get; set; }
    public decimal MaxCommission { get; set; }
}
