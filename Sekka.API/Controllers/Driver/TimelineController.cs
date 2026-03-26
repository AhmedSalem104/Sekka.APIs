using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/timeline")]
[Authorize]
public class TimelineController : ControllerBase
{
    private readonly ITimelineService _timelineService;

    public TimelineController(ITimelineService timelineService)
    {
        _timelineService = timelineService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("daily")]
    public async Task<IActionResult> GetDaily([FromQuery] DateOnly date)
    {
        var result = await _timelineService.GetDailyAsync(GetDriverId(), date);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetRange([FromQuery] DateOnly dateFrom, [FromQuery] DateOnly dateTo)
    {
        var result = await _timelineService.GetRangeAsync(GetDriverId(), dateFrom, dateTo);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("daily/filter")]
    public async Task<IActionResult> GetFiltered([FromQuery] DateOnly date, [FromQuery] List<TimelineEventType> eventTypes)
    {
        var result = await _timelineService.GetFilteredAsync(GetDriverId(), date, eventTypes);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
