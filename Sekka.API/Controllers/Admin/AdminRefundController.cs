using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/refunds")]
[Authorize(Roles = "Admin")]
public class AdminRefundController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRefunds([FromQuery] AdminRefundFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة المستردات")));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل الاسترداد")));

    [HttpPost("{id:guid}/approve")]
    public IActionResult Approve(Guid id, [FromBody] ReviewRefundDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("الموافقة على الاسترداد")));

    [HttpPost("{id:guid}/reject")]
    public IActionResult Reject(Guid id, [FromBody] ReviewRefundDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("رفض الاسترداد")));

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص المستردات")));

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminRefundFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصدير المستردات")));
}
