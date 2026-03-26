using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/disputes")]
[Authorize(Roles = "Admin")]
public class AdminDisputesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDisputes([FromQuery] AdminDisputeFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة النزاعات")));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل النزاع")));

    [HttpPost("{id:guid}/resolve")]
    public IActionResult Resolve(Guid id, [FromBody] ResolveDisputeDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("حل النزاع")));

    [HttpPost("{id:guid}/escalate")]
    public IActionResult Escalate(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصعيد النزاع")));

    [HttpPost("{id:guid}/assign")]
    public IActionResult Assign(Guid id, [FromBody] Guid adminId)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تعيين النزاع")));

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("ملخص النزاعات")));

    [HttpGet("export")]
    public IActionResult Export([FromQuery] AdminDisputeFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصدير النزاعات")));
}
