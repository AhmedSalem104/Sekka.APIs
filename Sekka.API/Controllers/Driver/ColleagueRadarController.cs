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
[Route("api/v{version:apiVersion}/colleague-radar")]
[Authorize]
public class ColleagueRadarController : ControllerBase
{
    private readonly IColleagueRadarService _radarService;

    public ColleagueRadarController(IColleagueRadarService radarService)
    {
        _radarService = radarService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 5)
        => ToActionResult(await _radarService.GetNearbyAsync(GetDriverId(), latitude, longitude, radiusKm));

    [HttpPost("help-requests")]
    public async Task<IActionResult> CreateHelpRequest([FromBody] CreateHelpRequestDto dto)
        => ToActionResult(await _radarService.CreateHelpRequestAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpGet("help-requests/nearby")]
    public async Task<IActionResult> GetNearbyHelpRequests(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10)
        => ToActionResult(await _radarService.GetNearbyHelpRequestsAsync(GetDriverId(), latitude, longitude, radiusKm));

    [HttpPost("help-requests/{requestId:guid}/respond")]
    public async Task<IActionResult> RespondToHelpRequest(Guid requestId)
        => ToActionResult(await _radarService.RespondToHelpRequestAsync(GetDriverId(), requestId));

    [HttpPost("help-requests/{requestId:guid}/resolve")]
    public async Task<IActionResult> ResolveHelpRequest(Guid requestId)
        => ToActionResult(await _radarService.ResolveHelpRequestAsync(GetDriverId(), requestId));

    [HttpGet("help-requests/my")]
    public async Task<IActionResult> GetMyHelpRequests()
        => ToActionResult(await _radarService.GetMyHelpRequestsAsync(GetDriverId()));

    [HttpPost("location")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        => ToActionResult(await _radarService.UpdateLocationAsync(GetDriverId(), dto));

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
