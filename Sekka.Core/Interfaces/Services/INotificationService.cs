using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Enums;

namespace Sekka.Core.Interfaces.Services;

public interface INotificationService
{
    Task<Result<PagedResult<NotificationDto>>> GetNotificationsAsync(Guid driverId, PaginationDto pagination);
    Task<Result<bool>> MarkAsReadAsync(Guid driverId, Guid id);
    Task<Result<bool>> MarkAllAsReadAsync(Guid driverId);
    Task<Result<int>> GetUnreadCountAsync(Guid driverId);
    Task CreateAndPushAsync(Guid driverId, NotificationType type, string title, string message,
        string? actionType = null, string? actionData = null,
        NotificationPriority priority = NotificationPriority.Medium);
}
