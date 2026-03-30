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
[Route("api/v{version:apiVersion}/admin/refunds")]
[Authorize(Roles = "Admin")]
public class AdminRefundController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminRefundController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetRefunds([FromQuery] AdminRefundFilterDto filter)
    {
        var query = _db.RefundRequests.AsNoTracking().AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(r => r.Status == filter.Status.Value);
        if (filter.DriverId.HasValue)
            query = query.Where(r => r.DriverId == filter.DriverId.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(r => r.CreatedAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(r => r.CreatedAt <= filter.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(r => (r.Description != null && r.Description.Contains(filter.Search))
                || (r.AdminNotes != null && r.AdminNotes.Contains(filter.Search)));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(r => r.Driver)
            .Include(r => r.Order)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(r => new
            {
                r.Id,
                r.OrderId,
                OrderNumber = r.Order.OrderNumber,
                r.DriverId,
                DriverName = r.Driver.Name,
                r.Amount,
                r.RefundReason,
                r.Status,
                r.Description,
                r.AdminNotes,
                r.ProcessedBy,
                r.ProcessedAt,
                r.CreatedAt
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
        var refund = await _db.RefundRequests.AsNoTracking()
            .Include(r => r.Driver)
            .Include(r => r.Order)
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.OrderId,
                OrderNumber = r.Order.OrderNumber,
                r.DriverId,
                DriverName = r.Driver.Name,
                r.Amount,
                r.RefundReason,
                r.Status,
                r.Description,
                r.AdminNotes,
                r.ProcessedBy,
                r.ProcessedAt,
                r.CreatedAt,
                r.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (refund is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.RefundNotFound));

        return Ok(ApiResponse<object>.Success(refund));
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ReviewRefundDto dto)
    {
        var refund = await _db.RefundRequests.FindAsync(id);
        if (refund is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.RefundNotFound));

        if (refund.Status != RefundStatus.Pending)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.RefundAlreadyProcessed));

        refund.Status = RefundStatus.Approved;
        refund.AdminNotes = dto.AdminNotes;
        refund.ProcessedBy = User.Identity?.Name;
        refund.ProcessedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            refund.Id,
            refund.Status,
            refund.Amount,
            refund.ProcessedAt
        }, SuccessMessages.RefundApproved));
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewRefundDto dto)
    {
        var refund = await _db.RefundRequests.FindAsync(id);
        if (refund is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.RefundNotFound));

        if (refund.Status != RefundStatus.Pending)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.RefundAlreadyProcessed));

        refund.Status = RefundStatus.Rejected;
        refund.AdminNotes = dto.AdminNotes;
        refund.ProcessedBy = User.Identity?.Name;
        refund.ProcessedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            refund.Id,
            refund.Status,
            refund.ProcessedAt
        }, SuccessMessages.RefundRejected));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.RefundRequests.CountAsync();
        var pending = await _db.RefundRequests.CountAsync(r => r.Status == RefundStatus.Pending);
        var approved = await _db.RefundRequests.CountAsync(r => r.Status == RefundStatus.Approved);
        var rejected = await _db.RefundRequests.CountAsync(r => r.Status == RefundStatus.Rejected);
        var processed = await _db.RefundRequests.CountAsync(r => r.Status == RefundStatus.Processed);

        var totalAmount = total > 0 ? await _db.RefundRequests.SumAsync(r => r.Amount) : 0;
        var approvedAmount = approved > 0
            ? await _db.RefundRequests.Where(r => r.Status == RefundStatus.Approved || r.Status == RefundStatus.Processed).SumAsync(r => r.Amount)
            : 0;

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Pending = pending,
            Approved = approved,
            Rejected = rejected,
            Processed = processed,
            TotalAmount = totalAmount,
            ApprovedAmount = approvedAmount
        }));
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminRefundFilterDto filter)
    {
        return Ok(ApiResponse<object>.Success(new
        {
            Message = "تم بدء عملية التصدير. سيتم إرسال الملف عبر الإشعارات",
            RequestedAt = DateTime.UtcNow
        }));
    }
}
