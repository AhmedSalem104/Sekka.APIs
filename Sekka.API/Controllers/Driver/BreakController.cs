using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Vehicle;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/breaks")]
[Authorize]
public class BreakController : ControllerBase
{
    private readonly IBreakService _breakService;

    public BreakController(IBreakService breakService)
    {
        _breakService = breakService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("start")]
    public async Task<IActionResult> StartBreak([FromBody] StartBreakDto dto)
        => ToActionResult(await _breakService.StartBreakAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpPost("end")]
    public async Task<IActionResult> EndBreak([FromBody] EndBreakDto dto)
        => ToActionResult(await _breakService.EndBreakAsync(GetDriverId(), dto));

    [HttpGet("suggestion")]
    public async Task<IActionResult> GetSuggestion()
        => ToActionResult(await _breakService.GetSuggestionAsync(GetDriverId()));

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] PaginationDto pagination)
        => ToActionResult(await _breakService.GetHistoryAsync(GetDriverId(), pagination));

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
