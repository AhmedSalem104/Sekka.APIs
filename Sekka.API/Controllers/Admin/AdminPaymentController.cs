using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/payments")]
[Authorize(Roles = "Admin")]
public class AdminPaymentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetPayments([FromQuery] AdminPaymentFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة طلبات الدفع")));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل طلب الدفع")));

    [HttpPost("{id:guid}/approve")]
    public IActionResult Approve(Guid id, [FromBody] ReviewPaymentDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("الموافقة على طلب الدفع")));

    [HttpPost("{id:guid}/reject")]
    public IActionResult Reject(Guid id, [FromBody] ReviewPaymentDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("رفض طلب الدفع")));

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص طلبات الدفع")));
}
