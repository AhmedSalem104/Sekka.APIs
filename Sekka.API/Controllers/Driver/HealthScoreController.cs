using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/health-score")]
[Authorize]
public class HealthScoreController : ControllerBase
{
    private readonly IHealthScoreService _healthScoreService;

    public HealthScoreController(IHealthScoreService healthScoreService)
    {
        _healthScoreService = healthScoreService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetHealthScore()
    {
        var result = await _healthScoreService.GetHealthScoreAsync(GetDriverId());
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("tips")]
    public async Task<IActionResult> GetTips()
    {
        var result = await _healthScoreService.GetHealthTipsAsync(GetDriverId());
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
