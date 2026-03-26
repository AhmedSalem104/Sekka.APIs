using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] CustomerFilterDto filter)
        => ToActionResult(await _customerService.GetCustomersAsync(GetDriverId(), filter));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => ToActionResult(await _customerService.GetByIdAsync(GetDriverId(), id));

    [HttpGet("by-phone/{phone}")]
    public async Task<IActionResult> GetByPhone(string phone)
        => ToActionResult(await _customerService.GetByPhoneAsync(GetDriverId(), phone));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto)
        => ToActionResult(await _customerService.UpdateAsync(GetDriverId(), id, dto));

    [HttpPost("{id:guid}/rate")]
    public async Task<IActionResult> Rate(Guid id, [FromBody] CreateRatingDto dto)
        => ToActionResult(await _customerService.RateAsync(GetDriverId(), id, dto), StatusCodes.Status201Created);

    [HttpPost("{id:guid}/block")]
    public async Task<IActionResult> Block(Guid id, [FromBody] BlockCustomerDto dto)
        => ToActionResult(await _customerService.BlockAsync(GetDriverId(), id, dto));

    [HttpPost("{id:guid}/unblock")]
    public async Task<IActionResult> Unblock(Guid id)
        => ToActionResult(await _customerService.UnblockAsync(GetDriverId(), id));

    [HttpGet("{id:guid}/orders")]
    public async Task<IActionResult> GetOrders(Guid id, [FromQuery] PaginationDto pagination)
        => ToActionResult(await _customerService.GetOrdersAsync(GetDriverId(), id, pagination));

    [HttpGet("{id:guid}/interests")]
    public async Task<IActionResult> GetInterests(Guid id)
        => ToActionResult(await _customerService.GetInterestsAsync(GetDriverId(), id));

    [HttpGet("{id:guid}/engagement")]
    public async Task<IActionResult> GetEngagement(Guid id)
        => ToActionResult(await _customerService.GetEngagementAsync(GetDriverId(), id));

    private IActionResult ToActionResult<T>(Result<T> result, int successCode = 200, string? message = null)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!, message));

        return result.Error!.Code switch
        {
            "NOT_FOUND" => NotFound(ApiResponse<T>.Fail(result.Error.Message)),
            "UNAUTHORIZED" => Unauthorized(ApiResponse<T>.Fail(result.Error.Message)),
            "CONFLICT" => Conflict(ApiResponse<T>.Fail(result.Error.Message)),
            _ => BadRequest(ApiResponse<T>.Fail(result.Error.Message))
        };
    }
}
