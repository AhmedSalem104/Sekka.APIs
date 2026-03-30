using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Persistence;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/customers")]
[Authorize(Roles = "Admin")]
public class AdminCustomersController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminCustomersController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] PaginationDto pagination, [FromQuery] string? searchTerm)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(c => c.Phone.Contains(searchTerm)
                || (c.Name != null && c.Name.Contains(searchTerm)));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(c => new
            {
                c.Id,
                c.Phone,
                c.Name,
                c.DriverId,
                c.AverageRating,
                c.TotalDeliveries,
                c.SuccessfulDeliveries,
                c.FailedDeliveries,
                c.IsBlocked,
                c.LastDeliveryDate,
                c.CreatedAt
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
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var customer = await _db.Customers.AsNoTracking()
            .Include(c => c.Addresses)
            .Include(c => c.Ratings)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Phone,
                c.Name,
                c.DriverId,
                c.AverageRating,
                c.TotalDeliveries,
                c.SuccessfulDeliveries,
                c.FailedDeliveries,
                c.IsBlocked,
                c.BlockReason,
                c.Notes,
                c.PreferredPaymentMethod,
                c.LastDeliveryDate,
                c.CreatedAt,
                AddressCount = c.Addresses.Count,
                RatingCount = c.Ratings.Count
            })
            .FirstOrDefaultAsync();

        if (customer is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.CustomerNotFound));

        return Ok(ApiResponse<object>.Success(customer));
    }

    [HttpPost("{id:guid}/block")]
    public async Task<IActionResult> BlockCustomer(Guid id, [FromBody] BlockCustomerDto dto)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.CustomerNotFound));

        if (customer.IsBlocked)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.CustomerAlreadyBlocked));

        customer.IsBlocked = true;
        customer.BlockReason = dto.Reason;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(true, SuccessMessages.CustomerBlocked));
    }

    [HttpPost("{id:guid}/unblock")]
    public async Task<IActionResult> UnblockCustomer(Guid id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.CustomerNotFound));

        if (!customer.IsBlocked)
            return BadRequest(ApiResponse<object>.Fail(ErrorMessages.CustomerNotBlocked));

        customer.IsBlocked = false;
        customer.BlockReason = null;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(true, SuccessMessages.CustomerUnblocked));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(c => c.CreatedAt >= dateFrom.Value.ToDateTime(TimeOnly.MinValue));
        if (dateTo.HasValue)
            query = query.Where(c => c.CreatedAt <= dateTo.Value.ToDateTime(TimeOnly.MaxValue));

        var total = await query.CountAsync();
        var blocked = await query.CountAsync(c => c.IsBlocked);
        var active = total - blocked;
        var avgRating = total > 0
            ? await query.AverageAsync(c => (double)c.AverageRating)
            : 0;
        var totalDeliveries = await query.SumAsync(c => c.TotalDeliveries);
        var successfulDeliveries = await query.SumAsync(c => c.SuccessfulDeliveries);
        var failedDeliveries = await query.SumAsync(c => c.FailedDeliveries);

        return Ok(ApiResponse<object>.Success(new
        {
            Total = total,
            Active = active,
            Blocked = blocked,
            AverageRating = Math.Round(avgRating, 2),
            TotalDeliveries = totalDeliveries,
            SuccessfulDeliveries = successfulDeliveries,
            FailedDeliveries = failedDeliveries
        }));
    }
}
