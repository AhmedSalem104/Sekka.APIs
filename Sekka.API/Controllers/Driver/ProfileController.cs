using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Profile;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetProfile()
        => ToActionResult(await _profileService.GetProfileAsync(GetDriverId()));

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        => ToActionResult(await _profileService.UpdateProfileAsync(GetDriverId(), dto), message: SuccessMessages.ProfileUpdated);

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var result = await _profileService.UploadProfileImageAsync(GetDriverId(), file.OpenReadStream(), file.FileName);
        return ToActionResult(result, message: SuccessMessages.ImageUploaded);
    }

    [HttpDelete("image")]
    public async Task<IActionResult> DeleteImage()
        => ToActionResult(await _profileService.DeleteProfileImageAsync(GetDriverId()));

    [HttpPut("license-image")]
    public async Task<IActionResult> UploadLicenseImage(IFormFile file)
    {
        var result = await _profileService.UploadLicenseImageAsync(GetDriverId(), file.OpenReadStream(), file.FileName);
        return ToActionResult(result, message: SuccessMessages.ImageUploaded);
    }

    [HttpGet("completion")]
    public async Task<IActionResult> GetCompletion()
        => ToActionResult(await _profileService.GetCompletionAsync(GetDriverId()));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        => ToActionResult(await _profileService.GetStatsAsync(GetDriverId(), fromDate, toDate));

    [HttpGet("badges")]
    public async Task<IActionResult> GetBadges()
        => ToActionResult(await _profileService.GetBadgesAsync(GetDriverId()));

    [HttpGet("activity-log")]
    public async Task<IActionResult> GetActivityLog([FromQuery] PaginationDto pagination)
        => ToActionResult(await _profileService.GetActivityLogAsync(GetDriverId(), pagination));

    [HttpGet("emergency-contacts")]
    public async Task<IActionResult> GetEmergencyContacts()
        => ToActionResult(await _profileService.GetEmergencyContactsAsync(GetDriverId()));

    [HttpPost("emergency-contacts")]
    public async Task<IActionResult> AddEmergencyContact([FromBody] CreateEmergencyContactDto dto)
        => ToActionResult(await _profileService.AddEmergencyContactAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpDelete("emergency-contacts/{id:guid}")]
    public async Task<IActionResult> DeleteEmergencyContact(Guid id)
        => ToActionResult(await _profileService.DeleteEmergencyContactAsync(GetDriverId(), id));

    [HttpGet("subscription")]
    public async Task<IActionResult> GetSubscription()
        => ToActionResult(await _profileService.GetSubscriptionAsync(GetDriverId()));

    [HttpPost("subscription/upgrade")]
    public async Task<IActionResult> UpgradeSubscription([FromBody] UpgradeSubscriptionDto dto)
        => ToActionResult(await _profileService.UpgradeSubscriptionAsync(GetDriverId(), dto));

    [HttpGet("achievements")]
    public async Task<IActionResult> GetAchievements()
        => ToActionResult(await _profileService.GetAchievementsAsync(GetDriverId()));

    [HttpGet("challenges")]
    public async Task<IActionResult> GetChallenges()
        => ToActionResult(await _profileService.GetChallengesAsync(GetDriverId()));

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
        => ToActionResult(await _profileService.GetLeaderboardAsync(GetDriverId()));

    [HttpGet("expenses")]
    public async Task<IActionResult> GetExpenses([FromQuery] ExpenseFilterDto filter)
        => ToActionResult(await _profileService.GetExpensesAsync(GetDriverId(), filter));

    [HttpPost("expenses")]
    public async Task<IActionResult> AddExpense([FromBody] CreateExpenseDto dto)
        => ToActionResult(await _profileService.AddExpenseAsync(GetDriverId(), dto), StatusCodes.Status201Created);

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
