using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/partners")]
[Authorize]
public class PartnerController : ControllerBase
{
    private readonly IPartnerService _partnerService;

    public PartnerController(IPartnerService partnerService)
    {
        _partnerService = partnerService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToActionResult(await _partnerService.GetAllAsync(GetDriverId()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartnerDto dto)
        => ToActionResult(await _partnerService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartnerDto dto)
        => ToActionResult(await _partnerService.UpdateAsync(GetDriverId(), id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _partnerService.DeleteAsync(GetDriverId(), id));

    [HttpGet("{id:guid}/orders")]
    public async Task<IActionResult> GetOrders(Guid id, [FromQuery] PaginationDto pagination)
        => ToActionResult(await _partnerService.GetOrdersAsync(GetDriverId(), id, pagination));

    [HttpGet("{id:guid}/pickup-points")]
    public async Task<IActionResult> GetPickupPoints(Guid id)
        => ToActionResult(await _partnerService.GetPickupPointsAsync(GetDriverId(), id));

    [HttpPost("{id:guid}/verification")]
    public async Task<IActionResult> SubmitVerification(Guid id, IFormFile file)
    {
        var result = await _partnerService.SubmitVerificationAsync(GetDriverId(), id, file.OpenReadStream(), file.FileName);
        return ToActionResult(result);
    }

    [HttpGet("{id:guid}/verification")]
    public async Task<IActionResult> GetVerificationStatus(Guid id)
        => ToActionResult(await _partnerService.GetVerificationStatusAsync(GetDriverId(), id));

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
