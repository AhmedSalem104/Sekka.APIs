using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/badge")]
[Authorize]
public class BadgeController : ControllerBase
{
    private readonly IBadgeService _badgeService;

    public BadgeController(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetBadge()
    {
        var result = await _badgeService.GetDigitalBadgeAsync(GetDriverId());
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("verify/{qrToken}")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyBadge(string qrToken)
    {
        var result = await _badgeService.VerifyBadgeAsync(qrToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
