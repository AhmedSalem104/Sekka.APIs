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
[Route("api/v{version:apiVersion}/invoices")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetInvoices([FromQuery] InvoiceFilterDto filter)
        => ToActionResult(await _invoiceService.GetInvoicesAsync(GetDriverId(), filter));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => ToActionResult(await _invoiceService.GetByIdAsync(GetDriverId(), id));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => ToActionResult(await _invoiceService.GetSummaryAsync(GetDriverId(), dateFrom ?? DateTime.UtcNow.AddDays(-30), dateTo ?? DateTime.UtcNow));

    [HttpGet("{id:guid}/pdf")]
    public IActionResult DownloadPdf(Guid id)
        => StatusCode(501, ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحميل PDF الفاتورة")));

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
