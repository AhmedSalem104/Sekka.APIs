using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders/recurring")]
[Authorize]
public class RecurringOrderController : ControllerBase
{
    private readonly IRecurringOrderService _recurringService;

    public RecurringOrderController(IRecurringOrderService recurringService)
    {
        _recurringService = recurringService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToActionResult(await _recurringService.GetRecurringOrdersAsync(GetDriverId()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecurringOrderDto dto)
        => ToActionResult(await _recurringService.CreateRecurringOrderAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecurringOrderDto dto)
        => ToActionResult(await _recurringService.UpdateRecurringOrderAsync(GetDriverId(), id, dto));

    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> Pause(Guid id)
        => ToActionResult(await _recurringService.PauseAsync(GetDriverId(), id));

    [HttpPost("{id:guid}/resume")]
    public async Task<IActionResult> Resume(Guid id)
        => ToActionResult(await _recurringService.ResumeAsync(GetDriverId(), id));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _recurringService.DeleteAsync(GetDriverId(), id));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
