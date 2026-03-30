using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Persistence;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/settlements")]
[Authorize(Roles = "Admin")]
public class AdminSettlementsController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminSettlementsController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettlements([FromQuery] AdminSettlementFilterDto filter)
    {
        var query = _db.Settlements.AsNoTracking().AsQueryable();

        if (filter.DriverId.HasValue)
            query = query.Where(s => s.DriverId == filter.DriverId.Value);
        if (filter.SettlementType.HasValue)
            query = query.Where(s => s.SettlementType == filter.SettlementType.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(s => s.SettledAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(s => s.SettledAt <= filter.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(s => s.Notes != null && s.Notes.Contains(filter.Search));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.SettledAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => new
            {
                s.Id,
                s.DriverId,
                s.PartnerId,
                s.Amount,
                s.SettlementType,
                s.OrderCount,
                s.Notes,
                s.ReceiptImageUrl,
                s.WhatsAppSent,
                s.SettledAt,
                s.CreatedAt
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
        var settlement = await _db.Settlements.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.DriverId,
                s.PartnerId,
                s.Amount,
                s.SettlementType,
                s.OrderCount,
                s.Notes,
                s.ReceiptImageUrl,
                s.WhatsAppSent,
                s.SettledAt,
                s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (settlement is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SettlementNotFound));

        return Ok(ApiResponse<object>.Success(settlement));
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var settlement = await _db.Settlements.FindAsync(id);
        if (settlement is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SettlementNotFound));

        // Settlement approved by admin — mark WhatsAppSent as confirmation signal
        settlement.WhatsAppSent = true;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            settlement.Id,
            settlement.Amount,
            Status = "Approved"
        }, SuccessMessages.SettlementApproved));
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string? reason)
    {
        var settlement = await _db.Settlements.FindAsync(id);
        if (settlement is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.SettlementNotFound));

        settlement.Notes = string.IsNullOrWhiteSpace(reason)
            ? settlement.Notes
            : $"{settlement.Notes} | رفض: {reason}";
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            settlement.Id,
            settlement.Amount,
            Status = "Rejected",
            Reason = reason
        }, SuccessMessages.SettlementRejected));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var query = _db.Settlements.AsNoTracking().AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(s => s.SettledAt >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(s => s.SettledAt <= dateTo.Value);

        var totalCount = await query.CountAsync();
        var totalAmount = totalCount > 0 ? await query.SumAsync(s => s.Amount) : 0;
        var totalOrders = totalCount > 0 ? await query.SumAsync(s => s.OrderCount) : 0;
        var uniqueDrivers = await query.Select(s => s.DriverId).Distinct().CountAsync();
        var uniquePartners = await query.Select(s => s.PartnerId).Distinct().CountAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            TotalSettlements = totalCount,
            TotalAmount = totalAmount,
            TotalOrders = totalOrders,
            UniqueDrivers = uniqueDrivers,
            UniquePartners = uniquePartners
        }));
    }
}
