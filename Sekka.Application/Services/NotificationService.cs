using AutoMapper;
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

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFirebaseService _firebaseService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper,
        IFirebaseService firebaseService, ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _firebaseService = firebaseService;
        _logger = logger;
    }

    public async Task<Result<PagedResult<NotificationDto>>> GetNotificationsAsync(Guid driverId, PaginationDto pagination)
    {
        var repo = _unitOfWork.GetRepository<Notification, Guid>();
        var spec = new NotificationsByDriverSpec(driverId, pagination);
        var items = await repo.ListAsync(spec);
        var countSpec = new NotificationsByDriverCountSpec(driverId);
        var total = await repo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<NotificationDto>>(items);
        return Result<PagedResult<NotificationDto>>.Success(
            new PagedResult<NotificationDto>(dtos, total, pagination.Page, pagination.PageSize));
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<Notification, Guid>();
        var notification = await repo.GetByIdAsync(id);

        if (notification == null || notification.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.NotificationNotFound);

        if (notification.IsRead)
            return Result<bool>.Success(true);

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        repo.Update(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Notification {NotificationId} marked as read by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkAllAsReadAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Notification, Guid>();
        var spec = new UnreadNotificationsByDriverSpec(driverId);
        var unread = await repo.ListAsync(spec);

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            repo.Update(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("All notifications marked as read for driver {DriverId}. Count: {Count}", driverId, unread.Count);

        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> GetUnreadCountAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Notification, Guid>();
        var spec = new UnreadNotificationsByDriverSpec(driverId);
        var count = await repo.CountAsync(spec);

        return Result<int>.Success(count);
    }

    public async Task CreateAndPushAsync(Guid driverId, NotificationType type, string title, string message,
        string? actionType = null, string? actionData = null,
        NotificationPriority priority = NotificationPriority.Medium)
    {
        try
        {
            // 1. Save to DB
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                DriverId = driverId,
                NotificationType = type,
                Title = title,
                Message = message,
                IsRead = false,
                ActionType = actionType,
                ActionData = actionData,
                Priority = priority
            };
            var repo = _unitOfWork.GetRepository<Notification, Guid>();
            await repo.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // 2. Send Firebase push
            var data = new Dictionary<string, string>
            {
                ["type"] = type.ToString(),
                ["notificationId"] = notification.Id.ToString()
            };
            if (actionType != null) data["actionType"] = actionType;
            if (actionData != null) data["actionData"] = actionData;

            await _firebaseService.SendPushAsync(driverId, title, message, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create/push notification for driver {DriverId}", driverId);
            // Don't throw — notifications should never break business flow
        }
    }
}

// ── Specifications ──

internal class NotificationsByDriverSpec : BaseSpecification<Notification>
{
    public NotificationsByDriverSpec(Guid driverId, PaginationDto pagination)
    {
        SetCriteria(n => n.DriverId == driverId);
        SetOrderByDescending(n => n.CreatedAt);
        ApplyPaging((pagination.Page - 1) * pagination.PageSize, pagination.PageSize);
    }
}

internal class NotificationsByDriverCountSpec : BaseSpecification<Notification>
{
    public NotificationsByDriverCountSpec(Guid driverId)
    {
        SetCriteria(n => n.DriverId == driverId);
    }
}

internal class UnreadNotificationsByDriverSpec : BaseSpecification<Notification>
{
    public UnreadNotificationsByDriverSpec(Guid driverId)
    {
        SetCriteria(n => n.DriverId == driverId && !n.IsRead);
    }
}
