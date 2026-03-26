using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wallet")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
        => ToActionResult(await _walletService.GetBalanceAsync(GetDriverId()));

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] WalletTransactionFilterDto filter)
        => ToActionResult(await _walletService.GetTransactionsAsync(GetDriverId(), filter));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
        => ToActionResult(await _walletService.GetSummaryAsync(GetDriverId()));

    [HttpGet("cash-status")]
    public async Task<IActionResult> GetCashStatus()
        => ToActionResult(await _walletService.GetCashStatusAsync(GetDriverId()));

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
