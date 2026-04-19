using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<Driver> _userManager;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<Driver> userManager, ILogger<ChatService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<PagedResult<ConversationDto>>> GetConversationsAsync(Guid driverId, PaginationDto pagination)
    {
        var repo = _unitOfWork.GetRepository<Conversation, Guid>();
        var spec = new ConversationsByDriverSpec(driverId, pagination);
        var items = await repo.ListAsync(spec);
        var countSpec = new ConversationsByDriverCountSpec(driverId);
        var total = await repo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<ConversationDto>>(items);
        return Result<PagedResult<ConversationDto>>.Success(
            new PagedResult<ConversationDto>(dtos, total, pagination.Page, pagination.PageSize));
    }

    public async Task<Result<ConversationDto>> CreateConversationAsync(Guid driverId, CreateConversationDto dto)
    {
        var conversationRepo = _unitOfWork.GetRepository<Conversation, Guid>();
        var messageRepo = _unitOfWork.GetRepository<ChatMessage, Guid>();

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            ChatType = dto.ChatType,
            Subject = dto.Subject,
            IsClosed = false,
            LastMessageAt = DateTime.UtcNow
        };

        await conversationRepo.AddAsync(conversation);

        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = driverId,
            SenderType = "Driver",
            Content = dto.InitialMessage,
            Status = MessageStatus.Sent
        };

        await messageRepo.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Conversation {ConversationId} created by driver {DriverId}", conversation.Id, driverId);

        return Result<ConversationDto>.Success(_mapper.Map<ConversationDto>(conversation));
    }

    public async Task<Result<PagedResult<ChatMessageDto>>> GetMessagesAsync(Guid driverId, Guid conversationId, PaginationDto pagination)
    {
        var conversationRepo = _unitOfWork.GetRepository<Conversation, Guid>();
        var conversation = await conversationRepo.GetByIdAsync(conversationId);

        if (conversation == null || conversation.DriverId != driverId)
            return Result<PagedResult<ChatMessageDto>>.NotFound(ErrorMessages.ConversationNotFound);

        var messageRepo = _unitOfWork.GetRepository<ChatMessage, Guid>();
        var spec = new MessagesByConversationSpec(conversationId, pagination);
        var items = await messageRepo.ListAsync(spec);
        var countSpec = new MessagesByConversationCountSpec(conversationId);
        var total = await messageRepo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<ChatMessageDto>>(items);
        await PopulateSenderNamesAsync(dtos);
        return Result<PagedResult<ChatMessageDto>>.Success(
            new PagedResult<ChatMessageDto>(dtos, total, pagination.Page, pagination.PageSize));
    }

    public async Task<Result<ChatMessageDto>> SendMessageAsync(Guid driverId, Guid conversationId, SendMessageDto dto)
    {
        var conversationRepo = _unitOfWork.GetRepository<Conversation, Guid>();
        var conversation = await conversationRepo.GetByIdAsync(conversationId);

        if (conversation == null || conversation.DriverId != driverId)
            return Result<ChatMessageDto>.NotFound(ErrorMessages.ConversationNotFound);

        if (conversation.IsClosed)
            return Result<ChatMessageDto>.BadRequest(ErrorMessages.ConversationClosed);

        var messageRepo = _unitOfWork.GetRepository<ChatMessage, Guid>();

        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = driverId,
            SenderType = "Driver",
            Content = dto.Content,
            AttachmentUrl = dto.AttachmentUrl,
            Status = MessageStatus.Sent
        };

        await messageRepo.AddAsync(message);

        conversation.LastMessageAt = DateTime.UtcNow;
        conversationRepo.Update(conversation);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Message sent in conversation {ConversationId} by driver {DriverId}", conversationId, driverId);

        var msgDto = _mapper.Map<ChatMessageDto>(message);
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        msgDto.SenderName = driver?.Name ?? "سائق";
        return Result<ChatMessageDto>.Success(msgDto);
    }

    public async Task<Result<bool>> CloseConversationAsync(Guid driverId, Guid conversationId)
    {
        var repo = _unitOfWork.GetRepository<Conversation, Guid>();
        var conversation = await repo.GetByIdAsync(conversationId);

        if (conversation == null || conversation.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ConversationNotFound);

        if (conversation.IsClosed)
            return Result<bool>.Success(true);

        conversation.IsClosed = true;
        conversation.ClosedAt = DateTime.UtcNow;
        repo.Update(conversation);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Conversation {ConversationId} closed by driver {DriverId}", conversationId, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkMessageReadAsync(Guid driverId, Guid messageId)
    {
        var messageRepo = _unitOfWork.GetRepository<ChatMessage, Guid>();
        var message = await messageRepo.GetByIdAsync(messageId);

        if (message == null)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        // Verify the driver owns the conversation
        var conversationRepo = _unitOfWork.GetRepository<Conversation, Guid>();
        var conversation = await conversationRepo.GetByIdAsync(message.ConversationId);

        if (conversation == null || conversation.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ConversationNotFound);

        if (message.Status == MessageStatus.Read)
            return Result<bool>.Success(true);

        message.Status = MessageStatus.Read;
        message.ReadAt = DateTime.UtcNow;
        messageRepo.Update(message);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> GetUnreadCountAsync(Guid driverId)
    {
        var messageRepo = _unitOfWork.GetRepository<ChatMessage, Guid>();
        var spec = new UnreadMessagesForDriverSpec(driverId);
        var count = await messageRepo.CountAsync(spec);

        return Result<int>.Success(count);
    }
    private async Task PopulateSenderNamesAsync(List<ChatMessageDto> messages)
    {
        var senderIds = messages.Where(m => string.IsNullOrEmpty(m.SenderName) || m.SenderName == null!)
            .Select(m => m.SenderId).Distinct().ToList();
        var nameCache = new Dictionary<Guid, string>();
        foreach (var id in senderIds)
        {
            var driver = await _userManager.FindByIdAsync(id.ToString());
            nameCache[id] = driver?.Name ?? "مستخدم";
        }
        foreach (var msg in messages)
        {
            if (string.IsNullOrEmpty(msg.SenderName) && nameCache.TryGetValue(msg.SenderId, out var name))
                msg.SenderName = name;
        }
    }
}

// ── Specifications ──

internal class ConversationsByDriverSpec : BaseSpecification<Conversation>
{
    public ConversationsByDriverSpec(Guid driverId, PaginationDto pagination)
    {
        SetCriteria(c => c.DriverId == driverId);
        SetOrderByDescending(c => c.LastMessageAt!);
        ApplyPaging((pagination.Page - 1) * pagination.PageSize, pagination.PageSize);
    }
}

internal class ConversationsByDriverCountSpec : BaseSpecification<Conversation>
{
    public ConversationsByDriverCountSpec(Guid driverId)
    {
        SetCriteria(c => c.DriverId == driverId);
    }
}

internal class MessagesByConversationSpec : BaseSpecification<ChatMessage>
{
    public MessagesByConversationSpec(Guid conversationId, PaginationDto pagination)
    {
        SetCriteria(m => m.ConversationId == conversationId);
        SetOrderByDescending(m => m.CreatedAt);
        ApplyPaging((pagination.Page - 1) * pagination.PageSize, pagination.PageSize);
    }
}

internal class MessagesByConversationCountSpec : BaseSpecification<ChatMessage>
{
    public MessagesByConversationCountSpec(Guid conversationId)
    {
        SetCriteria(m => m.ConversationId == conversationId);
    }
}

internal class UnreadMessagesForDriverSpec : BaseSpecification<ChatMessage>
{
    public UnreadMessagesForDriverSpec(Guid driverId)
    {
        SetCriteria(m => m.Conversation.DriverId == driverId && m.SenderType != "Driver" && m.Status != MessageStatus.Read);
        AddInclude(m => m.Conversation);
    }
}
