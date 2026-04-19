using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/partner")]
[Authorize]
public class PartnerPortalController : ControllerBase
{
    private readonly IPartnerPortalService _portalService;

    public PartnerPortalController(IPartnerPortalService portalService)
    {
        _portalService = portalService;
    }

    private Guid GetPartnerId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
        => ToActionResult(await _portalService.GetDashboardAsync(GetPartnerId()));

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders([FromQuery] PartnerOrdersFilterDto filter)
        => ToActionResult(await _portalService.GetOrdersAsync(GetPartnerId(), filter));

    [HttpGet("settlements")]
    public async Task<IActionResult> GetSettlements([FromQuery] SettlementFilterDto filter)
        => ToActionResult(await _portalService.GetSettlementsAsync(GetPartnerId(), filter));

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] PartnerSettingsDto dto)
        => ToActionResult(await _portalService.UpdateSettingsAsync(GetPartnerId(), dto));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
        => ToActionResult(await _portalService.GetStatsAsync(GetPartnerId(), dateFrom, dateTo));

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices([FromQuery] InvoiceFilterDto filter)
        => ToActionResult(await _portalService.GetInvoicesAsync(GetPartnerId(), filter));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            "NOT_IMPLEMENTED" => StatusCode(501, ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
