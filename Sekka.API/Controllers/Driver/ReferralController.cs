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
[Route("api/v{version:apiVersion}/referrals")]
[Authorize]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _referralService;

    public ReferralController(IReferralService referralService)
    {
        _referralService = referralService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("my-code")]
    public async Task<IActionResult> GetMyCode()
        => ToActionResult(await _referralService.GetMyCodeAsync(GetDriverId()));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
        => ToActionResult(await _referralService.GetStatsAsync(GetDriverId()));

    [HttpPost("apply")]
    public async Task<IActionResult> ApplyCode([FromBody] ApplyReferralCodeDto dto)
        => ToActionResult(await _referralService.ApplyCodeAsync(GetDriverId(), dto));

    [HttpGet]
    public async Task<IActionResult> GetMyReferrals()
        => ToActionResult(await _referralService.GetMyReferralsAsync(GetDriverId()));

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
