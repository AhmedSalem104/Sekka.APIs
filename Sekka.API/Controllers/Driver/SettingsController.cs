using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Settings;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IDriverPreferencesService _preferencesService;

    public SettingsController(IDriverPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get()
        => ToActionResult(await _preferencesService.GetAsync(GetDriverId()));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDriverPreferencesDto dto)
        => ToActionResult(await _preferencesService.UpdateAsync(GetDriverId(), dto), message: SuccessMessages.SettingsUpdated);

    [HttpPut("focus-mode")]
    public async Task<IActionResult> UpdateFocusMode([FromBody] UpdateFocusModeDto dto)
        => ToActionResult(await _preferencesService.UpdateFocusModeAsync(GetDriverId(), dto), message: SuccessMessages.SettingsUpdated);

    [HttpPut("quiet-hours")]
    public async Task<IActionResult> UpdateQuietHours([FromBody] UpdateQuietHoursDto dto)
        => ToActionResult(await _preferencesService.UpdateQuietHoursAsync(GetDriverId(), dto), message: SuccessMessages.SettingsUpdated);

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotifications([FromBody] UpdateNotificationPrefsDto dto)
        => ToActionResult(await _preferencesService.UpdateNotificationPrefsAsync(GetDriverId(), dto), message: SuccessMessages.SettingsUpdated);

    [HttpPut("cost-params")]
    public async Task<IActionResult> UpdateCostParams([FromBody] UpdateCostParamsDto dto)
        => ToActionResult(await _preferencesService.UpdateCostParamsAsync(GetDriverId(), dto), message: SuccessMessages.SettingsUpdated);

    [HttpGet("notification-channels")]
    public async Task<IActionResult> GetNotificationChannels()
        => ToActionResult(await _preferencesService.GetNotificationChannelsAsync(GetDriverId()));

    [HttpPut("notification-channels/{type}")]
    public async Task<IActionResult> UpdateNotificationChannel(NotificationType type, [FromBody] UpdateChannelPrefDto dto)
        => ToActionResult(await _preferencesService.UpdateNotificationChannelAsync(GetDriverId(), type, dto), message: SuccessMessages.SettingsUpdated);

    [HttpPut("notification-channels/bulk")]
    public async Task<IActionResult> UpdateNotificationChannelsBulk([FromBody] List<UpdateChannelPrefDto> dtos)
        => ToActionResult(await _preferencesService.UpdateNotificationChannelsBulkAsync(GetDriverId(), dtos), message: SuccessMessages.SettingsUpdated);

    [HttpPost("home-location")]
    public async Task<IActionResult> UpdateHomeLocation([FromBody] UpdateHomeLocationDto dto)
        => ToActionResult(await _preferencesService.UpdateHomeLocationAsync(GetDriverId(), dto), message: SuccessMessages.SettingsUpdated);

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            "NOT_IMPLEMENTED" => StatusCode(501, ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
