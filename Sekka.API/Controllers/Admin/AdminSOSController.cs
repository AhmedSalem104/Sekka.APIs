using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/sos")]
[Authorize(Roles = "Admin")]
public class AdminSOSController : ControllerBase
{
    [HttpGet("active")]
    public IActionResult GetActive()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("بلاغات الطوارئ النشطة")));

    [HttpGet]
    public IActionResult GetAll([FromQuery] PaginationDto pagination)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("جميع بلاغات الطوارئ")));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل بلاغ الطوارئ")));

    [HttpPut("{id:guid}/acknowledge")]
    public IActionResult Acknowledge(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تأكيد استلام البلاغ")));

    [HttpPut("{id:guid}/escalate")]
    public IActionResult Escalate(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصعيد البلاغ")));

    [HttpPut("{id:guid}/resolve")]
    public IActionResult Resolve(Guid id, [FromBody] ResolveSOSDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("حل بلاغ الطوارئ")));

    [HttpPut("{id:guid}/mark-false-alarm")]
    public IActionResult MarkFalseAlarm(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تعليم كبلاغ كاذب")));

    [HttpGet("stats")]
    public IActionResult GetStats()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إحصائيات الطوارئ")));

    [HttpGet("response-times")]
    public IActionResult GetResponseTimes()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("أوقات الاستجابة")));

    [HttpGet("heatmap")]
    public IActionResult GetHeatmap()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("خريطة حرارية للطوارئ")));
}
