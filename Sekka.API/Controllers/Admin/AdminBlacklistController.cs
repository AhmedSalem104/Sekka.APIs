using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/blacklist")]
[Authorize(Roles = "Admin")]
public class AdminBlacklistController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminBlacklistController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetBlacklist([FromQuery] PaginationDto pagination, [FromQuery] string? searchTerm)
    {
        var query = _db.CommunityBlacklist.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b => b.PhoneNumber.Contains(searchTerm));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(b => b.LastReportedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Select(b => new
            {
                b.PhoneNumber,
                b.ReportCount,
                b.SeverityScore,
                b.LastReportedAt,
                b.IsVerified
            }).ToList<object>(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }

    [HttpPost]
    public IActionResult AddToBlacklist([FromBody] BlockCustomerDto dto)
    {
        return BadRequest(ApiResponse<object>.Fail("يجب توفير رقم الهاتف. استخدم POST /verify/{phone} لتأكيد بلاغ موجود"));
    }

    [HttpPost("verify/{phone}")]
    public async Task<IActionResult> VerifyBlacklist(string phone)
    {
        var entry = await _db.CommunityBlacklist.FindAsync(phone);
        if (entry is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.BlacklistEntryNotFound));

        entry.IsVerified = true;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            entry.PhoneNumber,
            entry.IsVerified,
            entry.ReportCount,
            entry.SeverityScore
        }, SuccessMessages.BlacklistVerified));
    }

    [HttpDelete("{phone}")]
    public async Task<IActionResult> RemoveFromBlacklist(string phone)
    {
        var entry = await _db.CommunityBlacklist.FindAsync(phone);
        if (entry is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.BlacklistEntryNotFound));

        _db.CommunityBlacklist.Remove(entry);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(true, SuccessMessages.BlacklistRemoved));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.CommunityBlacklist.CountAsync();
        var verified = await _db.CommunityBlacklist.CountAsync(b => b.IsVerified);
        var unverified = await _db.CommunityBlacklist.CountAsync(b => !b.IsVerified);
        var avgSeverity = total > 0
            ? await _db.CommunityBlacklist.AverageAsync(b => (double)b.SeverityScore)
            : 0;

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Verified = verified,
            Unverified = unverified,
            AverageSeverity = Math.Round(avgSeverity, 2)
        }));
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetUnverifiedReports([FromQuery] PaginationDto pagination)
    {
        var query = _db.CommunityBlacklist.AsNoTracking()
            .Where(b => !b.IsVerified);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(b => b.SeverityScore)
            .ThenByDescending(b => b.ReportCount)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var result = new PagedResult<object>(
            items.Select(b => new
            {
                b.PhoneNumber,
                b.ReportCount,
                b.SeverityScore,
                b.LastReportedAt,
                b.IsVerified
            }).ToList<object>(),
            totalCount,
            pagination.Page,
            pagination.PageSize);

        return Ok(ApiResponse<object>.Success(result));
    }
}
