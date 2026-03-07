using Sekka.Core.Common;
using Sekka.Core.DTOs.Settings;
using Sekka.Core.Enums;

namespace Sekka.Core.Interfaces.Services;

public interface IDriverPreferencesService
{
    Task<Result<DriverPreferencesDto>> GetAsync(Guid driverId);
    Task<Result<DriverPreferencesDto>> UpdateAsync(Guid driverId, UpdateDriverPreferencesDto dto);
    Task<Result<bool>> UpdateFocusModeAsync(Guid driverId, UpdateFocusModeDto dto);
    Task<Result<bool>> UpdateQuietHoursAsync(Guid driverId, UpdateQuietHoursDto dto);
    Task<Result<bool>> UpdateNotificationPrefsAsync(Guid driverId, UpdateNotificationPrefsDto dto);
    Task<Result<bool>> UpdateCostParamsAsync(Guid driverId, UpdateCostParamsDto dto);
    Task<Result<List<NotificationChannelPrefDto>>> GetNotificationChannelsAsync(Guid driverId);
    Task<Result<NotificationChannelPrefDto>> UpdateNotificationChannelAsync(Guid driverId, NotificationType type, UpdateChannelPrefDto dto);
    Task<Result<List<NotificationChannelPrefDto>>> UpdateNotificationChannelsBulkAsync(Guid driverId, List<UpdateChannelPrefDto> dtos);
    Task<Result<bool>> UpdateHomeLocationAsync(Guid driverId, UpdateHomeLocationDto dto);
}
