using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.System;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/config")]
[Authorize]
public class AppConfigController : ControllerBase
{
    private readonly IAppConfigService _configService;

    public AppConfigController(IAppConfigService configService)
    {
        _configService = configService;
    }

    private Guid? GetDriverIdOrNull()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim) : null;
    }

    [HttpGet("check-version")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckVersion([FromQuery] AppPlatform platform, [FromQuery] string currentVersion)
        => ToActionResult(await _configService.CheckVersionAsync(platform, currentVersion));

    [HttpGet("notices")]
    public async Task<IActionResult> GetNotices()
        => ToActionResult(await _configService.GetNoticesAsync(GetDriverIdOrNull()));

    [HttpGet("features")]
    public async Task<IActionResult> GetFeatures()
        => ToActionResult(await _configService.GetFeaturesAsync(GetDriverIdOrNull()));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!));

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
