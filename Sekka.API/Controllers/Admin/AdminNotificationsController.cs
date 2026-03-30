using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Enums;
using Sekka.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/notifications")]
[Authorize(Roles = "Admin")]
public class AdminNotificationsController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminNotificationsController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastNotificationDto dto)
    {
        var driverIds = await _db.Drivers.AsNoTracking()
            .Where(d => d.IsActive)
            .Select(d => d.Id)
            .ToListAsync();

        var notifications = driverIds.Select(driverId => new Notification
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            NotificationType = NotificationType.SystemUpdate,
            Title = dto.Title,
            Message = dto.Message,
            Priority = (NotificationPriority)dto.Priority,
            IsRead = false
        }).ToList();

        _db.Notifications.AddRange(notifications);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            SentTo = driverIds.Count,
            dto.Title,
            dto.Message
        }, SuccessMessages.BroadcastSent));
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] PaginationDto pagination)
    {
        var query = _db.Notifications.AsNoTracking().AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(n => new
            {
                n.Id,
                n.DriverId,
                n.NotificationType,
                n.Title,
                n.Message,
                n.IsRead,
                n.Priority,
                n.ActionType,
                n.ActionData,
                n.ReadAt,
                n.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpPost("send-to-driver")]
    public async Task<IActionResult> SendToDriver([FromBody] SendToDriverDto dto)
    {
        var driverExists = await _db.Drivers.AnyAsync(d => d.Id == dto.DriverId);
        if (!driverExists)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DriverNotFound));

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            DriverId = dto.DriverId,
            NotificationType = NotificationType.SystemUpdate,
            Title = dto.Title,
            Message = dto.Message,
            Priority = (NotificationPriority)dto.Priority,
            IsRead = false
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            notification.Id,
            notification.DriverId,
            notification.Title,
            notification.Message,
            notification.CreatedAt
        }, SuccessMessages.MessageSent));
    }
}
