using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Interfaces.Services;
using System.Security.Claims;

namespace Sekka.API.Controllers.Driver;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid GetDriverId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] PaginationDto pagination)
        => ToActionResult(await _chatService.GetConversationsAsync(GetDriverId(), pagination));

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
        => ToActionResult(await _chatService.CreateConversationAsync(GetDriverId(), dto), StatusCodes.Status201Created, SuccessMessages.ConversationCreated);

    [HttpGet("conversations/{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, [FromQuery] PaginationDto pagination)
        => ToActionResult(await _chatService.GetMessagesAsync(GetDriverId(), id, pagination));

    [HttpPost("conversations/{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageDto dto)
        => ToActionResult(await _chatService.SendMessageAsync(GetDriverId(), id, dto), StatusCodes.Status201Created, SuccessMessages.MessageSent);

    [HttpPut("conversations/{id:guid}/close")]
    public async Task<IActionResult> CloseConversation(Guid id)
        => ToActionResult(await _chatService.CloseConversationAsync(GetDriverId(), id), message: SuccessMessages.ConversationClosedSuccess);

    [HttpPut("messages/{id:guid}/read")]
    public async Task<IActionResult> MarkMessageRead(Guid id)
        => ToActionResult(await _chatService.MarkMessageReadAsync(GetDriverId(), id), message: SuccessMessages.MessageReadSuccess);

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
        => ToActionResult(await _chatService.GetUnreadCountAsync(GetDriverId()));

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
