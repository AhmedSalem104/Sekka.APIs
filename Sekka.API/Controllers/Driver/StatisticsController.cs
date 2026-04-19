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
    public async Task<IActionResult> GetDaily([FromQuery] DateOnly? date)
    {
        var d = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        return ToActionResult(await _statisticsService.GetDailyAsync(GetDriverId(), d));
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeekly([FromQuery] DateOnly? weekStart)
    {
        var ws = weekStart ?? StartOfWeek(DateOnly.FromDateTime(DateTime.UtcNow));
        return ToActionResult(await _statisticsService.GetWeeklyAsync(GetDriverId(), ws));
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int? month, [FromQuery] int? year)
    {
        var m = month ?? DateTime.UtcNow.Month;
        var y = year ?? DateTime.UtcNow.Year;

        if (m < 1 || m > 12)
            return BadRequest(ApiResponse<MonthlyStatsDto>.Fail("الشهر لازم يكون من 1 لـ 12"));

        if (y < 2020 || y > 2030)
            return BadRequest(ApiResponse<MonthlyStatsDto>.Fail("السنة غير صالحة"));

        return ToActionResult(await _statisticsService.GetMonthlyAsync(GetDriverId(), m, y));
    }

    [HttpGet("heatmap")]
    public async Task<IActionResult> GetHeatmap([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var from = dateFrom ?? DateTime.UtcNow.AddDays(-30);
        var to = dateTo ?? DateTime.UtcNow;
        return ToActionResult(await _statisticsService.GetHeatmapAsync(GetDriverId(), from, to));
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
        => ToActionResult(await _statisticsService.GetDailyAsync(GetDriverId(), DateOnly.FromDateTime(DateTime.UtcNow)));

    private static DateOnly StartOfWeek(DateOnly date)
    {
        // Saturday-based week
        var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
        return date.AddDays(-diff);
    }

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
