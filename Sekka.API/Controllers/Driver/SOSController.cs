using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sos")]
[Authorize]
public class SOSController : ControllerBase
{
    private readonly ISOSService _sosService;

    public SOSController(ISOSService sosService)
    {
        _sosService = sosService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateSOSDto dto)
        => ToActionResult(await _sosService.ActivateAsync(GetDriverId(), dto), StatusCodes.Status201Created, SuccessMessages.SOSActivated);

    [HttpPost("{id:guid}/dismiss")]
    public async Task<IActionResult> Dismiss(Guid id)
        => ToActionResult(await _sosService.DismissAsync(GetDriverId(), id), message: SuccessMessages.SOSDismissed);

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveSOSDto dto)
        => ToActionResult(await _sosService.ResolveAsync(GetDriverId(), id, dto), message: SuccessMessages.SOSResolved);

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] PaginationDto pagination)
        => ToActionResult(await _sosService.GetHistoryAsync(GetDriverId(), pagination));

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
