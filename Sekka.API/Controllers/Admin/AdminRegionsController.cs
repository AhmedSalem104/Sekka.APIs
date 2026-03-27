using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.System;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/regions")]
[Authorize(Roles = "Admin")]
public class AdminRegionsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRegions()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("المناطق")));

    [HttpPost]
    public IActionResult Create([FromBody] CreateRegionDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إنشاء منطقة")));

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateRegionDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تحديث منطقة")));

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("حذف منطقة")));
}
