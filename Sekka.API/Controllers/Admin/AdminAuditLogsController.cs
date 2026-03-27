using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.System;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/audit-logs")]
[Authorize(Roles = "Admin")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AdminAuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] AuditLogFilterDto filter)
    {
        var result = await _auditLogService.GetLogsAsync(filter);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("entity/{entityType}/{entityId}")]
    public async Task<IActionResult> GetEntityLogs(string entityType, string entityId)
    {
        var result = await _auditLogService.GetEntityLogsAsync(entityType, entityId);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserLogs(Guid userId, [FromQuery] PaginationDto pagination)
    {
        var result = await _auditLogService.GetUserLogsAsync(userId, pagination);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var result = await _auditLogService.GetSummaryAsync(dateFrom, dateTo);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("actions")]
    public IActionResult GetAuditActions()
    {
        var actions = Enum.GetValues<Sekka.Core.Enums.AuditAction>()
            .Select(a => new { Value = (int)a, Name = a.ToString() })
            .ToList();
        return Ok(ApiResponse<object>.Success(actions));
    }

    [HttpGet("entities")]
    public async Task<IActionResult> GetTrackedEntities()
    {
        // Returns a summary of distinct entity types in audit logs
        var result = await _auditLogService.GetSummaryAsync(null, null);
        if (result.IsSuccess && result.Value != null)
        {
            var entityTypes = result.Value.TopEntities.Select(e => e.EntityType).ToList();
            return Ok(ApiResponse<object>.Success(entityTypes));
        }
        return BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
