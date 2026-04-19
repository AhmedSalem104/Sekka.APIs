using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IFirebaseService _firebaseService;

    public NotificationController(INotificationService notificationService, IFirebaseService firebaseService)
    {
        _notificationService = notificationService;
        _firebaseService = firebaseService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get paginated notifications for the authenticated driver
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] PaginationDto pagination)
        => ToActionResult(await _notificationService.GetNotificationsAsync(GetDriverId(), pagination));

    /// <summary>
    /// Mark a specific notification as read
    /// </summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
        => ToActionResult(await _notificationService.MarkAsReadAsync(GetDriverId(), id), message: SuccessMessages.NotificationRead);

    /// <summary>
    /// Mark all notifications as read for the authenticated driver
    /// </summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
        => ToActionResult(await _notificationService.MarkAllAsReadAsync(GetDriverId()), message: SuccessMessages.AllNotificationsRead);

    /// <summary>
    /// Get unread notifications count for the authenticated driver
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
        => ToActionResult(await _notificationService.GetUnreadCountAsync(GetDriverId()));

    /// <summary>
    /// Register FCM device token for push notifications (call on login/app start)
    /// </summary>
    [HttpPost("device-token")]
    public async Task<IActionResult> RegisterDeviceToken([FromBody] Core.DTOs.Communication.RegisterDeviceTokenDto dto)
        => ToActionResult(await _firebaseService.RegisterTokenAsync(GetDriverId(), dto.Token, dto.Platform), message: "Device token registered successfully");

    /// <summary>
    /// Remove FCM device token (call on logout)
    /// </summary>
    [HttpDelete("device-token")]
    public async Task<IActionResult> RemoveDeviceToken([FromBody] Core.DTOs.Communication.RemoveDeviceTokenDto dto)
        => ToActionResult(await _firebaseService.RemoveTokenAsync(GetDriverId(), dto.Token), message: "Device token removed successfully");

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
