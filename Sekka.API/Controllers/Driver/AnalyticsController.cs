using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("source-breakdown")]
    public async Task<IActionResult> GetSourceBreakdown([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => ToActionResult(await _analyticsService.GetSourceBreakdownAsync(GetDriverId(), dateFrom ?? DateTime.UtcNow.AddDays(-30), dateTo ?? DateTime.UtcNow));

    [HttpGet("customer-profitability")]
    public async Task<IActionResult> GetCustomerProfitability([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => ToActionResult(await _analyticsService.GetCustomerProfitabilityAsync(GetDriverId(), dateFrom ?? DateTime.UtcNow.AddDays(-30), dateTo ?? DateTime.UtcNow));

    [HttpGet("region-analysis")]
    public async Task<IActionResult> GetRegionAnalysis([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => ToActionResult(await _analyticsService.GetRegionAnalysisAsync(GetDriverId(), dateFrom ?? DateTime.UtcNow.AddDays(-30), dateTo ?? DateTime.UtcNow));

    [HttpGet("time-analysis")]
    public async Task<IActionResult> GetTimeAnalysis([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => ToActionResult(await _analyticsService.GetTimeAnalysisAsync(GetDriverId(), dateFrom ?? DateTime.UtcNow.AddDays(-30), dateTo ?? DateTime.UtcNow));

    [HttpGet("cancellation-report")]
    public async Task<IActionResult> GetCancellationReport([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => ToActionResult(await _analyticsService.GetCancellationReportAsync(GetDriverId(), dateFrom ?? DateTime.UtcNow.AddDays(-30), dateTo ?? DateTime.UtcNow));

    [HttpGet("profitability-trends")]
    public async Task<IActionResult> GetProfitabilityTrends([FromQuery] string period = "monthly")
        => ToActionResult(await _analyticsService.GetProfitabilityTrendsAsync(GetDriverId(), period));

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
