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

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/sos")]
[Authorize(Roles = "Admin")]
public class AdminSOSController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminSOSController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.SOSLogs.AsNoTracking()
            .Include(s => s.Driver)
            .Where(s => s.Status == SOSStatus.Active)
            .OrderByDescending(s => s.ActivatedAt)
            .Select(s => new
            {
                s.Id,
                s.DriverId,
                DriverName = s.Driver.Name,
                DriverPhone = s.Driver.PhoneNumber,
                s.Latitude,
                s.Longitude,
                s.Status,
                s.EscalationLevel,
                s.Notes,
                s.AdminNotified,
                s.ActivatedAt,
                s.AcknowledgedAt,
                s.AcknowledgedBy
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(items));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
    {
        var query = _db.SOSLogs.AsNoTracking().AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(s => s.Driver)
            .OrderByDescending(s => s.ActivatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(s => new
            {
                s.Id,
                s.DriverId,
                DriverName = s.Driver.Name,
                DriverPhone = s.Driver.PhoneNumber,
                s.Latitude,
                s.Longitude,
                s.Status,
                s.EscalationLevel,
                s.Notes,
                s.AdminNotified,
                s.ActivatedAt,
                s.AcknowledgedAt,
                s.ResolvedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sos = await _db.SOSLogs.AsNoTracking()
            .Include(s => s.Driver)
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.DriverId,
                DriverName = s.Driver.Name,
                DriverPhone = s.Driver.PhoneNumber,
                s.Latitude,
                s.Longitude,
                s.Status,
                s.EscalationLevel,
                s.NotifiedContacts,
                s.LocationSharedWithFamily,
                s.AdminNotified,
                s.Notes,
                s.ActivatedAt,
                s.AcknowledgedAt,
                s.AcknowledgedBy,
                s.ResolvedAt,
                s.ResolvedBy
            })
            .FirstOrDefaultAsync();

        if (sos is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SOSNotFound));

        return Ok(ApiResponse<object>.Success(sos));
    }

    [HttpPut("{id:guid}/acknowledge")]
    public async Task<IActionResult> Acknowledge(Guid id)
    {
        var sos = await _db.SOSLogs.FindAsync(id);
        if (sos is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SOSNotFound));

        if (sos.Status != SOSStatus.Active)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.SOSAlreadyResolved));

        sos.AcknowledgedAt = DateTime.UtcNow;
        sos.AcknowledgedBy = User.Identity?.Name;
        sos.AdminNotified = true;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            sos.Id,
            sos.AcknowledgedAt,
            sos.AcknowledgedBy
        }, SuccessMessages.SOSAcknowledged));
    }

    [HttpPut("{id:guid}/escalate")]
    public async Task<IActionResult> Escalate(Guid id)
    {
        var sos = await _db.SOSLogs.FindAsync(id);
        if (sos is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SOSNotFound));

        if (sos.Status != SOSStatus.Active)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.SOSAlreadyResolved));

        // Move to next escalation level
        if (sos.EscalationLevel < SOSEscalationLevel.PoliceNotified)
            sos.EscalationLevel = (SOSEscalationLevel)((int)sos.EscalationLevel + 1);

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            sos.Id,
            sos.EscalationLevel
        }, SuccessMessages.SOSEscalated));
    }

    [HttpPut("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveSOSDto dto)
    {
        var sos = await _db.SOSLogs.FindAsync(id);
        if (sos is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SOSNotFound));

        if (sos.Status != SOSStatus.Active)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.SOSAlreadyResolved));

        sos.Status = dto.WasFalseAlarm ? SOSStatus.FalseAlarm : SOSStatus.Resolved;
        sos.ResolvedAt = DateTime.UtcNow;
        sos.ResolvedBy = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(dto.Resolution))
            sos.Notes = string.IsNullOrWhiteSpace(sos.Notes)
                ? dto.Resolution
                : $"{sos.Notes}\n[حل]: {dto.Resolution}";

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            sos.Id,
            sos.Status,
            sos.ResolvedAt,
            sos.ResolvedBy
        }, SuccessMessages.SOSResolved));
    }

    [HttpPut("{id:guid}/mark-false-alarm")]
    public async Task<IActionResult> MarkFalseAlarm(Guid id)
    {
        var sos = await _db.SOSLogs.FindAsync(id);
        if (sos is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SOSNotFound));

        if (sos.Status != SOSStatus.Active)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.SOSAlreadyResolved));

        sos.Status = SOSStatus.FalseAlarm;
        sos.ResolvedAt = DateTime.UtcNow;
        sos.ResolvedBy = User.Identity?.Name;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            sos.Id,
            sos.Status,
            sos.ResolvedAt
        }, SuccessMessages.SOSResolved));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.SOSLogs.CountAsync();
        var active = await _db.SOSLogs.CountAsync(s => s.Status == SOSStatus.Active);
        var resolved = await _db.SOSLogs.CountAsync(s => s.Status == SOSStatus.Resolved);
        var dismissed = await _db.SOSLogs.CountAsync(s => s.Status == SOSStatus.Dismissed);
        var falseAlarm = await _db.SOSLogs.CountAsync(s => s.Status == SOSStatus.FalseAlarm);

        var critical = await _db.SOSLogs.CountAsync(s => s.EscalationLevel == SOSEscalationLevel.Critical);
        var policeNotified = await _db.SOSLogs.CountAsync(s => s.EscalationLevel == SOSEscalationLevel.PoliceNotified);

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Active = active,
            Resolved = resolved,
            Dismissed = dismissed,
            FalseAlarm = falseAlarm,
            Critical = critical,
            PoliceNotified = policeNotified
        }));
    }

    [HttpGet("response-times")]
    public async Task<IActionResult> GetResponseTimes()
    {
        var resolvedLogs = await _db.SOSLogs.AsNoTracking()
            .Where(s => s.ResolvedAt.HasValue)
            .Select(s => new
            {
                s.ActivatedAt,
                s.AcknowledgedAt,
                s.ResolvedAt
            })
            .ToListAsync();

        if (!resolvedLogs.Any())
            return Ok(ApiResponse<object>.Success(new
            {
                AverageAcknowledgeMinutes = 0.0,
                AverageResolveMinutes = 0.0,
                TotalResolved = 0
            }));

        var avgAcknowledge = resolvedLogs
            .Where(s => s.AcknowledgedAt.HasValue)
            .Select(s => (s.AcknowledgedAt!.Value - s.ActivatedAt).TotalMinutes)
            .DefaultIfEmpty(0)
            .Average();

        var avgResolve = resolvedLogs
            .Select(s => (s.ResolvedAt!.Value - s.ActivatedAt).TotalMinutes)
            .Average();

        return Ok(ApiResponse<object>.Success(new
        {
            AverageAcknowledgeMinutes = Math.Round(avgAcknowledge, 2),
            AverageResolveMinutes = Math.Round(avgResolve, 2),
            TotalResolved = resolvedLogs.Count
        }));
    }

    [HttpGet("heatmap")]
    public async Task<IActionResult> GetHeatmap()
    {
        var points = await _db.SOSLogs.AsNoTracking()
            .Select(s => new
            {
                s.Latitude,
                s.Longitude,
                s.Status,
                s.EscalationLevel,
                s.ActivatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            Points = points,
            TotalCount = points.Count
        }));
    }
}
