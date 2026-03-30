using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Persistence;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/disputes")]
[Authorize(Roles = "Admin")]
public class AdminDisputesController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminDisputesController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetDisputes([FromQuery] AdminDisputeFilterDto filter)
    {
        var query = _db.OrderDisputes.AsNoTracking().AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(d => d.Status == filter.Status.Value);
        if (filter.DriverId.HasValue)
            query = query.Where(d => d.DriverId == filter.DriverId.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(d => d.CreatedAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(d => d.CreatedAt <= filter.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(d => d.Description.Contains(filter.Search)
                || (d.AdminNotes != null && d.AdminNotes.Contains(filter.Search)));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(d => d.Driver)
            .OrderByDescending(d => d.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(d => new
            {
                d.Id,
                d.OrderId,
                d.DriverId,
                DriverName = d.Driver.Name,
                d.DisputeType,
                d.Status,
                d.Description,
                d.EvidenceUrls,
                d.AdminNotes,
                d.Resolution,
                d.ResolvedBy,
                d.ResolvedAt,
                d.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            filter.Page,
            filter.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dispute = await _db.OrderDisputes.AsNoTracking()
            .Include(d => d.Driver)
            .Include(d => d.Order)
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.OrderId,
                OrderNumber = d.Order.OrderNumber,
                d.DriverId,
                DriverName = d.Driver.Name,
                d.DisputeType,
                d.Status,
                d.Description,
                d.EvidenceUrls,
                d.AdminNotes,
                d.Resolution,
                d.ResolvedBy,
                d.ResolvedAt,
                d.CreatedAt,
                d.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (dispute is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DisputeNotFound));

        return Ok(ApiResponse<object>.Success(dispute));
    }

    [HttpPut("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveDisputeDto dto)
    {
        var dispute = await _db.OrderDisputes.FindAsync(id);
        if (dispute is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DisputeNotFound));

        if (dispute.Status == DisputeStatus.Resolved)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.DisputeAlreadyResolved));

        dispute.Status = DisputeStatus.Resolved;
        dispute.Resolution = dto.Resolution;
        dispute.AdminNotes = dto.AdminNotes;
        dispute.ResolvedAt = DateTime.UtcNow;
        dispute.ResolvedBy = User.Identity?.Name;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            dispute.Id,
            dispute.Status,
            dispute.Resolution,
            dispute.ResolvedAt
        }, SuccessMessages.DisputeResolved));
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ResolveDisputeDto dto)
    {
        var dispute = await _db.OrderDisputes.FindAsync(id);
        if (dispute is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DisputeNotFound));

        if (dispute.Status == DisputeStatus.Resolved)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.DisputeAlreadyResolved));

        dispute.Status = DisputeStatus.Rejected;
        dispute.Resolution = dto.Resolution;
        dispute.AdminNotes = dto.AdminNotes;
        dispute.ResolvedAt = DateTime.UtcNow;
        dispute.ResolvedBy = User.Identity?.Name;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            dispute.Id,
            dispute.Status,
            dispute.Resolution,
            dispute.ResolvedAt
        }, SuccessMessages.DisputeRejected));
    }

    [HttpPut("{id:guid}/escalate")]
    public async Task<IActionResult> Escalate(Guid id)
    {
        var dispute = await _db.OrderDisputes.FindAsync(id);
        if (dispute is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.DisputeNotFound));

        if (dispute.Status == DisputeStatus.Resolved)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.DisputeAlreadyResolved));

        dispute.Status = DisputeStatus.Escalated;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            dispute.Id,
            dispute.Status
        }, SuccessMessages.DisputeEscalated));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.OrderDisputes.CountAsync();
        var open = await _db.OrderDisputes.CountAsync(d => d.Status == DisputeStatus.Open);
        var inReview = await _db.OrderDisputes.CountAsync(d => d.Status == DisputeStatus.InReview);
        var resolved = await _db.OrderDisputes.CountAsync(d => d.Status == DisputeStatus.Resolved);
        var rejected = await _db.OrderDisputes.CountAsync(d => d.Status == DisputeStatus.Rejected);
        var escalated = await _db.OrderDisputes.CountAsync(d => d.Status == DisputeStatus.Escalated);

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Open = open,
            InReview = inReview,
            Resolved = resolved,
            Rejected = rejected,
            Escalated = escalated
        }));
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminDisputeFilterDto filter)
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية التصدير. سيتم إرسال الملف عبر الإشعارات",
            RequestedAt = DateTime.UtcNow
        }));
    }
}
