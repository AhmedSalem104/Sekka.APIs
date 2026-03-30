using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Persistence;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public SearchController(SekkaDbContext db)
    {
        _db = db;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Omni-search: searches orders, customers, and partners by query text
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> OmniSearch([FromQuery] string? q, [FromQuery] int limit = 10)
    {
        var driverId = GetDriverId();
        var query = q?.Trim() ?? "";

        if (string.IsNullOrEmpty(query) || query.Length < 2)
            return Ok(ApiResponse<OmniSearchResultDto>.Success(new OmniSearchResultDto()));

        // Search Orders (by order number, customer name/phone, delivery address)
        var orders = await _db.Orders
            .Where(o => o.DriverId == driverId && !o.IsDeleted &&
                (o.OrderNumber.Contains(query) ||
                 (o.CustomerName != null && o.CustomerName.Contains(query)) ||
                 (o.CustomerPhone != null && o.CustomerPhone.Contains(query)) ||
                 o.DeliveryAddress.Contains(query)))
            .OrderByDescending(o => o.CreatedAt)
            .Take(limit)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.CustomerName,
                o.CustomerPhone,
                o.DeliveryAddress,
                o.Amount,
                o.Status,
                o.CreatedAt
            })
            .ToListAsync();

        // Search Customers (by name, phone)
        var customers = await _db.Customers
            .Where(c => c.DriverId == driverId &&
                ((c.Name != null && c.Name.Contains(query)) ||
                 c.Phone.Contains(query)))
            .OrderByDescending(c => c.TotalDeliveries)
            .Take(limit)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Phone,
                c.TotalDeliveries,
                c.CreatedAt
            })
            .ToListAsync();

        // Search Partners (by name, phone, address)
        var partners = await _db.Partners
            .Where(p => p.DriverId == driverId &&
                (p.Name.Contains(query) ||
                 (p.Phone != null && p.Phone.Contains(query)) ||
                 (p.Address != null && p.Address.Contains(query))))
            .Take(limit)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Phone,
                p.Address,
                p.PartnerType,
                p.IsActive
            })
            .ToListAsync();

        var result = new OmniSearchResultDto
        {
            Orders = orders.Cast<object>().ToList(),
            Customers = customers.Cast<object>().ToList(),
            Partners = partners.Cast<object>().ToList(),
            TotalResults = orders.Count + customers.Count + partners.Count
        };

        return Ok(ApiResponse<OmniSearchResultDto>.Success(result));
    }
}
