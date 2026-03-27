using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/savings-circles")]
[Authorize]
public class SavingsCircleController : ControllerBase
{
    private readonly ISavingsCircleService _circleService;

    public SavingsCircleController(ISavingsCircleService circleService)
    {
        _circleService = circleService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCircleDto dto)
        => ToActionResult(await _circleService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] PaginationDto pagination)
        => ToActionResult(await _circleService.GetAvailableAsync(pagination));

    [HttpGet("{circleId:guid}")]
    public async Task<IActionResult> GetById(Guid circleId)
        => ToActionResult(await _circleService.GetByIdAsync(circleId));

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCircles()
        => ToActionResult(await _circleService.GetMyCirclesAsync(GetDriverId()));

    [HttpPost("{circleId:guid}/join")]
    public async Task<IActionResult> Join(Guid circleId)
        => ToActionResult(await _circleService.JoinAsync(GetDriverId(), circleId));

    [HttpPost("{circleId:guid}/leave")]
    public async Task<IActionResult> Leave(Guid circleId)
        => ToActionResult(await _circleService.LeaveAsync(GetDriverId(), circleId));

    [HttpPost("{circleId:guid}/pay")]
    public async Task<IActionResult> MakePayment(Guid circleId)
        => ToActionResult(await _circleService.MakePaymentAsync(GetDriverId(), circleId), StatusCodes.Status201Created);

    [HttpGet("{circleId:guid}/payments")]
    public async Task<IActionResult> GetPayments(Guid circleId)
        => ToActionResult(await _circleService.GetPaymentsAsync(circleId));

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
