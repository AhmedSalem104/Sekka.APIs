using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.DTOs.Common;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/segments")]
[Authorize(Roles = "Admin")]
public class AdminSegmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSegments([FromQuery] SegmentFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("قائمة الشرائح")));

    [HttpGet("{id:guid}")]
    public IActionResult GetSegment(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تفاصيل الشريحة")));

    [HttpPost]
    public IActionResult CreateSegment([FromBody] CreateSegmentDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إنشاء شريحة")));

    [HttpPut("{id:guid}")]
    public IActionResult UpdateSegment(Guid id, [FromBody] UpdateSegmentDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تعديل شريحة")));

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteSegment(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("حذف شريحة")));

    [HttpPost("{id:guid}/refresh")]
    public IActionResult RefreshSegment(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحديث الشريحة")));

    [HttpGet("{id:guid}/members")]
    public IActionResult GetMembers(Guid id, [FromQuery] PaginationDto pagination)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("أعضاء الشريحة")));

    [HttpPost("{id:guid}/members/{customerId:guid}")]
    public IActionResult AddMember(Guid id, Guid customerId)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إضافة عضو للشريحة")));

    [HttpDelete("{id:guid}/members/{customerId:guid}")]
    public IActionResult RemoveMember(Guid id, Guid customerId)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إزالة عضو من الشريحة")));

    [HttpGet("analytics")]
    public IActionResult GetAnalytics()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحليلات الشرائح")));
}
