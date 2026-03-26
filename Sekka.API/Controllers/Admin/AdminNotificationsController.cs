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
[Route("api/v{version:apiVersion}/admin/notifications")]
[Authorize(Roles = "Admin")]
public class AdminNotificationsController : ControllerBase
{
    [HttpPost("broadcast")]
    public IActionResult Broadcast([FromBody] BroadcastNotificationDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إشعارات جماعية")));

    [HttpGet("history")]
    public IActionResult GetHistory([FromQuery] PaginationDto pagination)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("سجل الإشعارات")));

    [HttpPost("send-to-driver")]
    public IActionResult SendToDriver([FromBody] SendToDriverDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إرسال إشعار لسائق")));
}
