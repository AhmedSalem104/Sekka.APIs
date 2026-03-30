using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Enums;
using Sekka.Persistence;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/partners")]
[Authorize(Roles = "Admin")]
public class AdminPartnersController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminPartnersController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetPartners([FromQuery] PaginationDto pagination, [FromQuery] string? searchTerm)
    {
        var query = _db.Partners.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => p.Name.Contains(searchTerm)
                || (p.Phone != null && p.Phone.Contains(searchTerm)));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new
            {
                p.Id,
                p.DriverId,
                p.Name,
                p.PartnerType,
                p.Phone,
                p.Address,
                p.CommissionType,
                p.CommissionValue,
                p.IsActive,
                p.VerificationStatus,
                p.CreatedAt
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
    public async Task<IActionResult> GetPartner(Guid id)
    {
        var partner = await _db.Partners.AsNoTracking()
            .Include(p => p.PickupPoints)
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.DriverId,
                p.Name,
                p.PartnerType,
                p.Phone,
                p.Address,
                p.CommissionType,
                p.CommissionValue,
                p.DefaultPaymentMethod,
                p.Color,
                p.LogoUrl,
                p.ReceiptHeader,
                p.IsActive,
                p.VerificationStatus,
                p.VerificationDocumentUrl,
                p.VerificationNote,
                p.VerifiedAt,
                p.CreatedAt,
                PickupPointCount = p.PickupPoints.Count
            })
            .FirstOrDefaultAsync();

        if (partner is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PartnerNotFound));

        return Ok(ApiResponse<object>.Success(partner));
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var partner = await _db.Partners.FindAsync(id);
        if (partner is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PartnerNotFound));

        partner.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(true, "تم تفعيل الشريك بنجاح"));
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var partner = await _db.Partners.FindAsync(id);
        if (partner is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PartnerNotFound));

        partner.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(true, "تم تعطيل الشريك بنجاح"));
    }

    [HttpPut("{id:guid}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] PartnerVerificationDto dto)
    {
        var partner = await _db.Partners.FindAsync(id);
        if (partner is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PartnerNotFound));

        partner.VerificationStatus = dto.Status;
        partner.VerificationNote = dto.Note;
        partner.VerificationDocumentUrl = dto.DocumentUrl ?? partner.VerificationDocumentUrl;
        partner.VerifiedAt = dto.Status == VerificationStatus.Verified ? DateTime.UtcNow : partner.VerifiedAt;

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            partner.Id,
            partner.VerificationStatus,
            partner.VerificationNote,
            partner.VerifiedAt
        }, SuccessMessages.PartnerVerified));
    }

    [HttpGet("{id:guid}/orders")]
    public async Task<IActionResult> GetOrders(Guid id, [FromQuery] PaginationDto pagination)
    {
        var partnerExists = await _db.Partners.AnyAsync(p => p.Id == id);
        if (!partnerExists)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PartnerNotFound));

        var query = _db.Orders.AsNoTracking()
            .Where(o => o.PartnerId == id);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.CustomerName,
                o.CustomerPhone,
                o.Amount,
                o.Status,
                o.DeliveryAddress,
                o.CreatedAt,
                o.DeliveredAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("{id:guid}/settlements")]
    public async Task<IActionResult> GetSettlements(Guid id, [FromQuery] PaginationDto pagination)
    {
        var partnerExists = await _db.Partners.AnyAsync(p => p.Id == id);
        if (!partnerExists)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.PartnerNotFound));

        var query = _db.Settlements.AsNoTracking()
            .Where(s => s.PartnerId == id);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.SettledAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(s => new
            {
                s.Id,
                s.DriverId,
                s.Amount,
                s.SettlementType,
                s.OrderCount,
                s.Notes,
                s.WhatsAppSent,
                s.SettledAt,
                s.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
    {
        var query = _db.Partners.AsNoTracking().AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(p => p.CreatedAt >= dateFrom.Value.ToDateTime(TimeOnly.MinValue));
        if (dateTo.HasValue)
            query = query.Where(p => p.CreatedAt <= dateTo.Value.ToDateTime(TimeOnly.MaxValue));

        var total = await query.CountAsync();
        var active = await query.CountAsync(p => p.IsActive);
        var inactive = total - active;
        var verified = await query.CountAsync(p => p.VerificationStatus == VerificationStatus.Verified);
        var pending = await query.CountAsync(p => p.VerificationStatus == VerificationStatus.Pending);
        var rejected = await query.CountAsync(p => p.VerificationStatus == VerificationStatus.Rejected);

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Active = active,
            Inactive = inactive,
            Verified = verified,
            PendingVerification = pending,
            Rejected = rejected
        }));
    }

    [HttpGet("pending-verification")]
    public async Task<IActionResult> GetPendingVerification([FromQuery] PaginationDto pagination)
    {
        var query = _db.Partners.AsNoTracking()
            .Where(p => p.VerificationStatus == VerificationStatus.Pending);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new
            {
                p.Id,
                p.DriverId,
                p.Name,
                p.Phone,
                p.PartnerType,
                p.VerificationStatus,
                p.VerificationDocumentUrl,
                p.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Cast<object>().ToList(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }
}
