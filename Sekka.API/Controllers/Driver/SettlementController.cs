using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settlements")]
[Authorize]
public class SettlementController : ControllerBase
{
    private readonly ISettlementService _settlementService;

    public SettlementController(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetSettlements([FromQuery] SettlementFilterDto filter)
        => ToActionResult(await _settlementService.GetSettlementsAsync(GetDriverId(), filter));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSettlementDto dto)
        => ToActionResult(await _settlementService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created, SuccessMessages.SettlementCreated);

    [HttpGet("partner/{partnerId:guid}/balance")]
    public async Task<IActionResult> GetPartnerBalance(Guid partnerId)
        => ToActionResult(await _settlementService.GetPartnerBalanceAsync(GetDriverId(), partnerId));

    [HttpGet("daily-summary")]
    public async Task<IActionResult> GetDailySummary()
        => ToActionResult(await _settlementService.GetDailySummaryAsync(GetDriverId()));

    [HttpPost("{id:guid}/receipt")]
    public async Task<IActionResult> UploadReceipt(Guid id, IFormFile file)
    {
        var result = await _settlementService.UploadReceiptAsync(GetDriverId(), id, file.OpenReadStream(), file.FileName);
        return ToActionResult(result, message: SuccessMessages.ReceiptUploaded);
    }

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
