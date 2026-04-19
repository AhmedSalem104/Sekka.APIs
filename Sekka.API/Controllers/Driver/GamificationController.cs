using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/gamification")]
[Authorize]
public class GamificationController : ControllerBase
{
    private readonly IGamificationService _gamificationService;

    public GamificationController(IGamificationService gamificationService)
    {
        _gamificationService = gamificationService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("challenges")]
    public async Task<IActionResult> GetActiveChallenges()
        => ToActionResult(await _gamificationService.GetActiveChallengesAsync(GetDriverId()));

    [HttpGet("achievements")]
    public async Task<IActionResult> GetAchievements()
        => ToActionResult(await _gamificationService.GetAchievementsAsync(GetDriverId()));

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard([FromQuery] string period = "monthly")
        => ToActionResult(await _gamificationService.GetLeaderboardAsync(GetDriverId(), period));

    [HttpPost("challenges/{challengeId:guid}/claim")]
    public async Task<IActionResult> ClaimReward(Guid challengeId)
        => ToActionResult(await _gamificationService.ClaimRewardAsync(GetDriverId(), challengeId));

    [HttpGet("points/history")]
    public async Task<IActionResult> GetPointsHistory([FromQuery] PaginationDto pagination)
        => ToActionResult(await _gamificationService.GetPointsHistoryAsync(GetDriverId(), pagination));

    [HttpGet("points/total")]
    public async Task<IActionResult> GetTotalPoints()
        => ToActionResult(await _gamificationService.GetTotalPointsAsync(GetDriverId()));

    [HttpGet("level")]
    public async Task<IActionResult> GetCurrentLevel()
        => ToActionResult(await _gamificationService.GetCurrentLevelAsync(GetDriverId()));

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
