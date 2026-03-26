using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/settlements")]
[Authorize(Roles = "Admin")]
public class AdminSettlementsController : ControllerBase
{
    private readonly ISettlementService _settlementService;

    public AdminSettlementsController(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    [HttpGet]
    public IActionResult GetSettlements([FromQuery] AdminSettlementFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة التسويات")));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل التسوية")));

    [HttpPost("{id:guid}/approve")]
    public IActionResult Approve(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("الموافقة على التسوية")));

    [HttpPost("{id:guid}/reject")]
    public IActionResult Reject(Guid id, [FromBody] string? reason)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("رفض التسوية")));

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص التسويات")));
}
