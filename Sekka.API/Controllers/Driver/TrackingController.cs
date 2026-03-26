using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tracking")]
[AllowAnonymous]
public class TrackingController : ControllerBase
{
    private readonly ITrackingLinkService _trackingService;

    public TrackingController(ITrackingLinkService trackingService)
    {
        _trackingService = trackingService;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetTrackingPage(string code)
    {
        var result = await _trackingService.GetTrackingPageAsync(code);
        if (result.IsSuccess)
            return Ok(ApiResponse<object>.Success(result.Value!));
        return NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
