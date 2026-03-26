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
[Route("api/v{version:apiVersion}/admin/time-slots")]
[Authorize(Roles = "Admin")]
public class AdminTimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public AdminTimeSlotsController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTimeSlots([FromQuery] DateOnly date, [FromQuery] Guid? regionId)
    {
        var result = await _timeSlotService.GetTimeSlotsAsync(date, regionId);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimeSlotDto dto)
    {
        var result = await _timeSlotService.CreateTimeSlotAsync(dto);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result.Value!, SuccessMessages.TimeSlotCreated))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimeSlotDto dto)
    {
        var result = await _timeSlotService.UpdateTimeSlotAsync(id, dto);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.TimeSlotUpdated))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _timeSlotService.DeleteTimeSlotAsync(id);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!, SuccessMessages.TimeSlotDeleted))
            : NotFound(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpPost("generate-week")]
    public async Task<IActionResult> GenerateWeek([FromBody] GenerateWeekSlotsDto dto)
    {
        var result = await _timeSlotService.GenerateWeekSlotsAsync(dto);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] DateOnly? dateFrom, [FromQuery] DateOnly? dateTo)
    {
        var result = await _timeSlotService.GetStatsAsync(dateFrom, dateTo);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Success(result.Value!))
            : BadRequest(ApiResponse<object>.Fail(result.Error!.Message));
    }
}
