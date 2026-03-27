using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
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
        => Stub("إعدادات النظام");

    [HttpGet("settings/{key}")]
    public IActionResult GetConfig(string key)
        => Stub("إعداد النظام");

    [HttpPut("settings/{key}")]
    public IActionResult UpdateConfig(string key, [FromBody] UpdateConfigDto dto)
        => Stub("تحديث إعداد النظام");

    // ══════════════════════════════════════
    // App Versions
    // ══════════════════════════════════════

    [HttpGet("versions")]
    public IActionResult GetVersions()
        => Stub("إصدارات التطبيق");

    [HttpPost("versions")]
    public IActionResult CreateVersion([FromBody] CreateAppVersionDto dto)
        => Stub("إنشاء إصدار");

    [HttpPut("versions/{id:guid}")]
    public IActionResult UpdateVersion(Guid id, [FromBody] UpdateAppVersionDto dto)
        => Stub("تحديث إصدار");

    [HttpDelete("versions/{id:guid}")]
    public IActionResult DeleteVersion(Guid id)
        => Stub("حذف إصدار");

    [HttpPut("versions/{id:guid}/force-update")]
    public IActionResult SetForceUpdate(Guid id, [FromQuery] bool force)
        => Stub("إجبار التحديث");

    // ══════════════════════════════════════
    // Maintenance Windows
    // ══════════════════════════════════════

    [HttpGet("maintenance")]
    public IActionResult GetMaintenanceWindows()
        => Stub("نوافذ الصيانة");

    [HttpPost("maintenance")]
    public IActionResult CreateMaintenanceWindow([FromBody] CreateMaintenanceWindowDto dto)
        => Stub("إنشاء نافذة صيانة");

    [HttpPut("maintenance/{id:guid}")]
    public IActionResult UpdateMaintenanceWindow(Guid id, [FromBody] UpdateMaintenanceWindowDto dto)
        => Stub("تحديث نافذة صيانة");

    [HttpDelete("maintenance/{id:guid}")]
    public IActionResult DeleteMaintenanceWindow(Guid id)
        => Stub("حذف نافذة صيانة");

    [HttpPost("maintenance/instant")]
    public IActionResult StartInstantMaintenance([FromBody] InstantMaintenanceDto dto)
        => Stub("صيانة فورية");

    // ══════════════════════════════════════
    // Feature Flags
    // ══════════════════════════════════════

    [HttpGet("feature-flags")]
    public IActionResult GetFeatureFlags()
        => Stub("أعلام الميزات");

    [HttpPost("feature-flags")]
    public IActionResult CreateFeatureFlag([FromBody] CreateFeatureFlagDto dto)
        => Stub("إنشاء علم ميزة");

    [HttpPut("feature-flags/{id:guid}")]
    public IActionResult UpdateFeatureFlag(Guid id, [FromBody] UpdateFeatureFlagDto dto)
        => Stub("تحديث علم ميزة");

    [HttpDelete("feature-flags/{id:guid}")]
    public IActionResult DeleteFeatureFlag(Guid id)
        => Stub("حذف علم ميزة");

    [HttpPut("feature-flags/{id:guid}/toggle")]
    public IActionResult ToggleFeatureFlag(Guid id)
        => Stub("تبديل علم ميزة");

    // ══════════════════════════════════════
    // Commission Settings
    // ══════════════════════════════════════

    [HttpGet("commissions")]
    public IActionResult GetCommissions()
        => Stub("إعدادات العمولة");

    [HttpPut("commissions")]
    public IActionResult UpdateCommissions([FromBody] CommissionSettingsDto dto)
        => Stub("تحديث إعدادات العمولة");

    // ── Helper ──
    private IActionResult Stub(string feature)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment(feature)));
}
