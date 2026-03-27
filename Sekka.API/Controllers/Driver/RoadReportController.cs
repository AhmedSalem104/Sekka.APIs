using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/road-reports")]
[Authorize]
public class RoadReportController : ControllerBase
{
    private readonly IRoadReportService _roadReportService;

    public RoadReportController(IRoadReportService roadReportService)
    {
        _roadReportService = roadReportService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoadReportDto dto)
        => ToActionResult(await _roadReportService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10)
        => ToActionResult(await _roadReportService.GetNearbyAsync(latitude, longitude, radiusKm));

    [HttpPost("{reportId:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid reportId, [FromQuery] bool isConfirmed = true)
        => ToActionResult(await _roadReportService.ConfirmAsync(GetDriverId(), reportId, isConfirmed));

    [HttpGet("{reportId:guid}")]
    public async Task<IActionResult> GetById(Guid reportId)
        => ToActionResult(await _roadReportService.GetByIdAsync(reportId));

    [HttpGet("my")]
    public async Task<IActionResult> GetMyReports()
        => ToActionResult(await _roadReportService.GetMyReportsAsync(GetDriverId()));

    [HttpPost("{reportId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid reportId)
        => ToActionResult(await _roadReportService.DeactivateAsync(GetDriverId(), reportId));

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
