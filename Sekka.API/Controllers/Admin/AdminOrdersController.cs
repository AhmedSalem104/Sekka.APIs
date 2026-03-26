using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Services;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IAutoAssignmentService _autoAssignService;

    public AdminOrdersController(IOrderService orderService, IAutoAssignmentService autoAssignService)
    {
        _orderService = orderService;
        _autoAssignService = autoAssignService;
    }

    [HttpGet]
    public IActionResult GetOrders([FromQuery] AdminOrderFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إدارة الطلبات للأدمن")));

    [HttpGet("board")]
    public IActionResult GetBoard()
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("لوحة كانبان")));

    [HttpPost]
    public IActionResult AdminCreate([FromBody] AdminCreateOrderDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("إنشاء طلب أدمن")));

    [HttpPost("{id:guid}/assign")]
    public IActionResult Assign(Guid id, [FromBody] AssignOrderDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تعيين طلب")));

    [HttpPost("auto-distribute")]
    public IActionResult AutoDistribute([FromBody] AutoDistributeDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("التوزيع التلقائي")));

    [HttpGet("{id:guid}/timeline")]
    public IActionResult GetTimeline(Guid id)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("الخط الزمني")));

    [HttpPut("{id:guid}/override-status")]
    public IActionResult OverrideStatus(Guid id, [FromBody] OverrideStatusDto dto)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تغيير الحالة يدوياً")));

    [HttpGet("export")]
    public IActionResult Export([FromQuery] ExportFilterDto filter)
        => BadRequest(ApiResponse<object>.Fail(ErrorMessages.FeatureUnderDevelopment("تصدير الطلبات")));

    [HttpPost("{id:guid}/auto-assign")]
    public async Task<IActionResult> AutoAssign(Guid id, [FromBody] AssignmentConfigDto config)
    {
        var result = await _autoAssignService.AutoAssignAsync(id, config);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.OrderAssigned))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("{id:guid}/suggested-drivers")]
    public async Task<IActionResult> GetSuggestedDrivers(Guid id)
    {
        var result = await _autoAssignService.GetSuggestedDriversAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
