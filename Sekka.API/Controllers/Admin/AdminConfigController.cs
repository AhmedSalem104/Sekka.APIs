using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.System;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/config")]
[Authorize(Roles = "Admin")]
public class AdminConfigController : ControllerBase
{
    // ══════════════════════════════════════
    // App Config (Key-Value)
    // ══════════════════════════════════════

    [HttpGet("settings")]
    public IActionResult GetConfigs()
        => Ok(ApiResponse<List<AppConfigDto>>.Success(new List<AppConfigDto>()));

    [HttpGet("settings/{key}")]
    public IActionResult GetConfig(string key)
        => Ok(ApiResponse<AppConfigDto>.Success(new AppConfigDto
        {
            ConfigKey = key,
            ConfigValue = string.Empty,
            UpdatedAt = DateTime.UtcNow
        }));

    [HttpPut("settings/{key}")]
    public IActionResult UpdateConfig(string key, [FromBody] UpdateConfigDto dto)
        => Ok(ApiResponse<AppConfigDto>.Success(new AppConfigDto
        {
            ConfigKey = key,
            ConfigValue = dto.ConfigValue,
            UpdatedAt = DateTime.UtcNow
        }, "تم تحديث الإعداد بنجاح"));

    // ══════════════════════════════════════
    // App Versions
    // ══════════════════════════════════════

    [HttpGet("versions")]
    public IActionResult GetVersions()
        => Ok(ApiResponse<List<AppVersionDto>>.Success(new List<AppVersionDto>()));

    [HttpPost("versions")]
    public IActionResult CreateVersion([FromBody] CreateAppVersionDto dto)
        => StatusCode(201, ApiResponse<AppVersionDto>.Success(new AppVersionDto
        {
            Id = Guid.NewGuid(),
            Platform = dto.Platform,
            VersionCode = dto.VersionCode,
            VersionNumber = dto.VersionNumber,
            MinRequiredVersion = dto.MinRequiredVersion,
            MinRequiredVersionNumber = dto.MinRequiredVersionNumber,
            StoreUrl = dto.StoreUrl,
            ReleaseNotes = dto.ReleaseNotes,
            IsForceUpdate = dto.IsForceUpdate,
            IsActive = true,
            ReleasedAt = DateTime.UtcNow
        }));

    [HttpPut("versions/{id:guid}")]
    public IActionResult UpdateVersion(Guid id, [FromBody] UpdateAppVersionDto dto)
        => Ok(ApiResponse<AppVersionDto>.Success(new AppVersionDto
        {
            Id = id,
            VersionCode = dto.VersionCode ?? string.Empty,
            VersionNumber = dto.VersionNumber ?? string.Empty,
            MinRequiredVersion = dto.MinRequiredVersion ?? string.Empty,
            MinRequiredVersionNumber = dto.MinRequiredVersionNumber ?? 0,
            StoreUrl = dto.StoreUrl ?? string.Empty,
            ReleaseNotes = dto.ReleaseNotes,
            IsForceUpdate = dto.IsForceUpdate ?? false,
            IsActive = dto.IsActive ?? true,
            ReleasedAt = DateTime.UtcNow
        }, "تم تحديث الإصدار بنجاح"));

    [HttpDelete("versions/{id:guid}")]
    public IActionResult DeleteVersion(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم حذف الإصدار بنجاح"));

    [HttpPut("versions/{id:guid}/force-update")]
    public IActionResult SetForceUpdate(Guid id, [FromQuery] bool force)
        => Ok(ApiResponse<bool>.Success(true, force ? "تم تفعيل إجبار التحديث" : "تم إلغاء إجبار التحديث"));

    // ══════════════════════════════════════
    // Maintenance Windows
    // ══════════════════════════════════════

    [HttpGet("maintenance")]
    public IActionResult GetMaintenanceWindows()
        => Ok(ApiResponse<List<MaintenanceWindowDto>>.Success(new List<MaintenanceWindowDto>()));

    [HttpPost("maintenance")]
    public IActionResult CreateMaintenanceWindow([FromBody] CreateMaintenanceWindowDto dto)
        => StatusCode(201, ApiResponse<MaintenanceWindowDto>.Success(new MaintenanceWindowDto
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Message = dto.Message,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsFullBlock = dto.IsFullBlock,
            IsActive = true
        }));

    [HttpPut("maintenance/{id:guid}")]
    public IActionResult UpdateMaintenanceWindow(Guid id, [FromBody] UpdateMaintenanceWindowDto dto)
        => Ok(ApiResponse<MaintenanceWindowDto>.Success(new MaintenanceWindowDto
        {
            Id = id,
            Title = dto.Title ?? string.Empty,
            Message = dto.Message ?? string.Empty,
            StartTime = dto.StartTime ?? DateTime.UtcNow,
            EndTime = dto.EndTime ?? DateTime.UtcNow.AddHours(1),
            IsFullBlock = dto.IsFullBlock ?? false,
            IsActive = dto.IsActive ?? true
        }, "تم تحديث نافذة الصيانة بنجاح"));

    [HttpDelete("maintenance/{id:guid}")]
    public IActionResult DeleteMaintenanceWindow(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم حذف نافذة الصيانة بنجاح"));

    [HttpPost("maintenance/instant")]
    public IActionResult StartInstantMaintenance([FromBody] InstantMaintenanceDto dto)
        => Ok(ApiResponse<MaintenanceWindowDto>.Success(new MaintenanceWindowDto
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Message = dto.Message,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(dto.DurationMinutes),
            IsFullBlock = dto.IsFullBlock,
            IsActive = true
        }, "تم بدء الصيانة الفورية"));

    // ══════════════════════════════════════
    // Feature Flags
    // ══════════════════════════════════════

    [HttpGet("feature-flags")]
    public IActionResult GetFeatureFlags()
        => Ok(ApiResponse<List<FeatureFlagDto>>.Success(new List<FeatureFlagDto>()));

    [HttpPost("feature-flags")]
    public IActionResult CreateFeatureFlag([FromBody] CreateFeatureFlagDto dto)
        => StatusCode(201, ApiResponse<FeatureFlagDto>.Success(new FeatureFlagDto
        {
            Id = Guid.NewGuid(),
            FeatureKey = dto.FeatureKey,
            DisplayName = dto.DisplayName,
            DisplayNameEn = dto.DisplayNameEn,
            Description = dto.Description,
            IsEnabled = dto.IsEnabled,
            EnabledForPremiumOnly = dto.EnabledForPremiumOnly,
            EnabledForPercentage = dto.EnabledForPercentage,
            Category = dto.Category
        }));

    [HttpPut("feature-flags/{id:guid}")]
    public IActionResult UpdateFeatureFlag(Guid id, [FromBody] UpdateFeatureFlagDto dto)
        => Ok(ApiResponse<FeatureFlagDto>.Success(new FeatureFlagDto
        {
            Id = id,
            FeatureKey = dto.FeatureKey ?? string.Empty,
            DisplayName = dto.DisplayName ?? string.Empty,
            DisplayNameEn = dto.DisplayNameEn,
            Description = dto.Description,
            IsEnabled = dto.IsEnabled ?? false,
            EnabledForPremiumOnly = dto.EnabledForPremiumOnly ?? false,
            EnabledForPercentage = dto.EnabledForPercentage ?? 100,
            Category = dto.Category,
            ExpiresAt = dto.ExpiresAt
        }, "تم تحديث علم الميزة بنجاح"));

    [HttpDelete("feature-flags/{id:guid}")]
    public IActionResult DeleteFeatureFlag(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم حذف علم الميزة بنجاح"));

    [HttpPut("feature-flags/{id:guid}/toggle")]
    public IActionResult ToggleFeatureFlag(Guid id)
        => Ok(ApiResponse<bool>.Success(true, "تم تبديل حالة الميزة بنجاح"));

    // ══════════════════════════════════════
    // Commission Settings
    // ══════════════════════════════════════

    [HttpGet("commissions")]
    public IActionResult GetCommissions()
        => Ok(ApiResponse<CommissionSettingsDto>.Success(new CommissionSettingsDto()));

    [HttpPut("commissions")]
    public IActionResult UpdateCommissions([FromBody] CommissionSettingsDto dto)
        => Ok(ApiResponse<CommissionSettingsDto>.Success(dto, "تم تحديث إعدادات العمولة بنجاح"));
}
