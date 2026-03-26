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
[Route("api/v{version:apiVersion}/statistics")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("daily")]
    public async Task<IActionResult> GetDaily([FromQuery] DateOnly date)
        => ToActionResult(await _statisticsService.GetDailyAsync(GetDriverId(), date));

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeekly([FromQuery] DateOnly weekStart)
        => ToActionResult(await _statisticsService.GetWeeklyAsync(GetDriverId(), weekStart));

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int month, [FromQuery] int year)
        => ToActionResult(await _statisticsService.GetMonthlyAsync(GetDriverId(), month, year));

    [HttpGet("heatmap")]
    public async Task<IActionResult> GetHeatmap([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        => ToActionResult(await _statisticsService.GetHeatmapAsync(GetDriverId(), dateFrom, dateTo));

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
        => ToActionResult(await _statisticsService.GetDailyAsync(GetDriverId(), DateOnly.FromDateTime(DateTime.UtcNow)));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
