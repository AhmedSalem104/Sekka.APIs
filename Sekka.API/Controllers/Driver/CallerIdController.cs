using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/caller-id")]
[Authorize]
public class CallerIdController : ControllerBase
{
    private readonly ICallerIdService _callerIdService;

    public CallerIdController(ICallerIdService callerIdService)
    {
        _callerIdService = callerIdService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("lookup/{phone}")]
    public async Task<IActionResult> Lookup(string phone)
        => ToActionResult(await _callerIdService.LookupAsync(GetDriverId(), phone));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCallerIdNoteDto dto)
        => ToActionResult(await _callerIdService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCallerIdNoteDto dto)
        => ToActionResult(await _callerIdService.UpdateAsync(GetDriverId(), id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _callerIdService.DeleteAsync(GetDriverId(), id));

    [HttpGet("truecaller/{phone}")]
    public async Task<IActionResult> TruecallerLookup(string phone)
        => ToActionResult(await _callerIdService.TruecallerLookupAsync(phone));

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
