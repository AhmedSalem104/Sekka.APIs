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
[Route("api/v{version:apiVersion}/payment-requests")]
[Authorize]
public class PaymentRequestController : ControllerBase
{
    private readonly IPaymentRequestService _paymentRequestService;

    public PaymentRequestController(IPaymentRequestService paymentRequestService)
    {
        _paymentRequestService = paymentRequestService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetRequests([FromQuery] PaymentRequestFilterDto filter)
        => ToActionResult(await _paymentRequestService.GetRequestsAsync(GetDriverId(), filter));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => ToActionResult(await _paymentRequestService.GetByIdAsync(GetDriverId(), id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto dto)
        => ToActionResult(await _paymentRequestService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created, SuccessMessages.PaymentRequestCreated);

    [HttpPost("{id:guid}/proof")]
    public async Task<IActionResult> UploadProof(Guid id, IFormFile file)
    {
        var result = await _paymentRequestService.UploadProofAsync(GetDriverId(), id, file.OpenReadStream(), file.FileName);
        return ToActionResult(result, message: SuccessMessages.ProofUploaded);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
        => ToActionResult(await _paymentRequestService.CancelAsync(GetDriverId(), id), message: SuccessMessages.PaymentRequestCancelled);

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
