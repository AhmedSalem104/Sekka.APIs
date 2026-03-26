using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Route;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/routes")]
[Authorize]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("optimize")]
    public async Task<IActionResult> Optimize([FromBody] OptimizeRouteDto dto)
        => ToActionResult(await _routeService.OptimizeRouteAsync(GetDriverId(), dto), message: SuccessMessages.RouteOptimized);

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
        => ToActionResult(await _routeService.GetActiveRouteAsync(GetDriverId()));

    [HttpPut("{id:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderRouteDto dto)
        => ToActionResult(await _routeService.ReorderRouteAsync(GetDriverId(), id, dto), message: SuccessMessages.RouteReordered);

    [HttpPost("{id:guid}/add-order")]
    public async Task<IActionResult> AddOrder(Guid id, [FromBody] AddOrderToRouteDto dto)
        => ToActionResult(await _routeService.AddOrderToRouteAsync(GetDriverId(), id, dto));

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
        => ToActionResult(await _routeService.CompleteRouteAsync(GetDriverId(), id));

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
