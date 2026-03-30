using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Persistence;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/payments")]
[Authorize(Roles = "Admin")]
public class AdminPaymentController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminPaymentController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments([FromQuery] AdminPaymentFilterDto filter)
    {
        var query = _db.PaymentRequests.AsNoTracking().AsQueryable();

        if (filter.DriverId.HasValue)
            query = query.Where(p => p.DriverId == filter.DriverId.Value);
        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(p => p.CreatedAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(p => p.CreatedAt <= filter.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.ReferenceCode.Contains(filter.Search)
                || (p.SenderName != null && p.SenderName.Contains(filter.Search))
                || (p.SenderPhone != null && p.SenderPhone.Contains(filter.Search)));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new
            {
                p.Id,
                p.DriverId,
                p.PaymentPurpose,
                p.Amount,
                p.PaymentMethod,
                p.ReferenceCode,
                p.ProofImageUrl,
                p.SenderPhone,
                p.SenderName,
                p.Notes,
                p.Status,
                p.AdminNotes,
                p.ReviewedAt,
                p.CreatedAt
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
        var payment = await _db.PaymentRequests.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.DriverId,
                p.PaymentPurpose,
                p.Amount,
                p.PaymentMethod,
                p.ReferenceCode,
                p.ProofImageUrl,
                p.SenderPhone,
                p.SenderName,
                p.Notes,
                p.Status,
                p.AdminId,
                p.AdminNotes,
                p.ReviewedAt,
                p.RelatedEntityId,
                p.RelatedEntityType,
                p.ExpiresAt,
                p.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (payment is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PaymentRequestNotFound));

        return Ok(ApiResponse<object>.Success(payment));
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ReviewPaymentDto dto)
    {
        var payment = await _db.PaymentRequests.FindAsync(id);
        if (payment is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PaymentRequestNotFound));

        if (payment.Status != PaymentRequestStatus.Pending)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.PaymentRequestAlreadyReviewed));

        payment.Status = PaymentRequestStatus.Approved;
        payment.AdminNotes = dto.AdminNotes;
        payment.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            payment.Id,
            payment.Status,
            payment.ReviewedAt
        }, SuccessMessages.PaymentRequestApproved));
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewPaymentDto dto)
    {
        var payment = await _db.PaymentRequests.FindAsync(id);
        if (payment is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PaymentRequestNotFound));

        if (payment.Status != PaymentRequestStatus.Pending)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.PaymentRequestAlreadyReviewed));

        payment.Status = PaymentRequestStatus.Rejected;
        payment.AdminNotes = dto.AdminNotes;
        payment.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            payment.Id,
            payment.Status,
            payment.ReviewedAt
        }, SuccessMessages.PaymentRequestRejected));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending([FromQuery] PaginationDto? pagination)
    {
        var page = pagination?.Page ?? 1;
        var pageSize = pagination?.PageSize ?? 20;

        var query = _db.PaymentRequests.AsNoTracking()
            .Where(p => p.Status == PaymentRequestStatus.Pending);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.DriverId,
                p.PaymentPurpose,
                p.Amount,
                p.PaymentMethod,
                p.ReferenceCode,
                p.SenderName,
                p.SenderPhone,
                p.ExpiresAt,
                p.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            page,
            pageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var query = _db.PaymentRequests.AsNoTracking().AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(p => p.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(p => p.CreatedAt <= dateTo.Value);

        var total = await query.CountAsync();
        var pending = await query.CountAsync(p => p.Status == PaymentRequestStatus.Pending);
        var approved = await query.CountAsync(p => p.Status == PaymentRequestStatus.Approved);
        var rejected = await query.CountAsync(p => p.Status == PaymentRequestStatus.Rejected);
        var expired = await query.CountAsync(p => p.Status == PaymentRequestStatus.Expired);
        var totalAmount = total > 0 ? await query.SumAsync(p => p.Amount) : 0;
        var approvedAmount = approved > 0
            ? await query.Where(p => p.Status == PaymentRequestStatus.Approved).SumAsync(p => p.Amount)
            : 0;

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Pending = pending,
            Approved = approved,
            Rejected = rejected,
            Expired = expired,
            TotalAmount = totalAmount,
            ApprovedAmount = approvedAmount
        }));
    }
}
