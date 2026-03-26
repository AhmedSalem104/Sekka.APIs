using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/message-templates")]
[Authorize]
public class MessageTemplateController : ControllerBase
{
    private readonly IMessageTemplateService _templateService;

    public MessageTemplateController(IMessageTemplateService templateService)
    {
        _templateService = templateService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetTemplates()
        => ToActionResult(await _templateService.GetTemplatesAsync(GetDriverId()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMessageTemplateDto dto)
        => ToActionResult(await _templateService.CreateAsync(GetDriverId(), dto), StatusCodes.Status201Created, SuccessMessages.TemplateCreated);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMessageTemplateDto dto)
        => ToActionResult(await _templateService.UpdateAsync(GetDriverId(), id, dto), message: SuccessMessages.TemplateUpdated);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _templateService.DeleteAsync(GetDriverId(), id), message: SuccessMessages.TemplateDeleted);

    [HttpPost("{id:guid}/use")]
    public async Task<IActionResult> RecordUsage(Guid id)
        => ToActionResult(await _templateService.RecordUsageAsync(GetDriverId(), id));

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
