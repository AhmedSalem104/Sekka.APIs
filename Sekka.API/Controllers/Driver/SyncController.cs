using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Sync;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sync")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("push")]
    public async Task<IActionResult> Push([FromBody] SyncPushDto dto)
        => ToActionResult(await _syncService.PushAsync(GetDriverId(), dto), message: SuccessMessages.SyncCompleted);

    [HttpGet("pull")]
    public async Task<IActionResult> Pull([FromQuery] DateTime? lastSyncTimestamp)
        => ToActionResult(await _syncService.PullAsync(GetDriverId(), lastSyncTimestamp));

    [HttpPost("resolve-conflict")]
    public async Task<IActionResult> ResolveConflict([FromBody] SyncConflictResolutionDto dto)
        => ToActionResult(await _syncService.ResolveConflictAsync(GetDriverId(), dto));

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
        => ToActionResult(await _syncService.GetStatusAsync(GetDriverId()));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
