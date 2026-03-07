using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Settings;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class DriverPreferencesService : IDriverPreferencesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DriverPreferencesService> _logger;

    public DriverPreferencesService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DriverPreferencesService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DriverPreferencesDto>> GetAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<DriverPreferences, Guid>();
        var prefs = (await repo.ListAsync(new DriverPreferencesByDriverSpec(driverId))).FirstOrDefault();

        if (prefs == null)
        {
            prefs = new DriverPreferences { DriverId = driverId };
            await repo.AddAsync(prefs);
            await _unitOfWork.SaveChangesAsync();
        }

        return Result<DriverPreferencesDto>.Success(_mapper.Map<DriverPreferencesDto>(prefs));
    }

    public async Task<Result<DriverPreferencesDto>> UpdateAsync(Guid driverId, UpdateDriverPreferencesDto dto)
    {
        var repo = _unitOfWork.GetRepository<DriverPreferences, Guid>();
        var prefs = (await repo.ListAsync(new DriverPreferencesByDriverSpec(driverId))).FirstOrDefault();

        if (prefs == null)
            return Result<DriverPreferencesDto>.NotFound(ErrorMessages.SettingsNotFound);

        if (dto.Theme.HasValue) prefs.Theme = dto.Theme.Value;
        if (dto.Language != null) prefs.Language = dto.Language;
        if (dto.NumberFormat.HasValue) prefs.NumberFormat = dto.NumberFormat.Value;
        if (dto.FocusModeAutoTrigger.HasValue) prefs.FocusModeAutoTrigger = dto.FocusModeAutoTrigger.Value;
        if (dto.FocusModeSpeedThreshold.HasValue) prefs.FocusModeSpeedThreshold = dto.FocusModeSpeedThreshold.Value;
        if (dto.TextToSpeechEnabled.HasValue) prefs.TextToSpeechEnabled = dto.TextToSpeechEnabled.Value;
        if (dto.HapticFeedback.HasValue) prefs.HapticFeedback = dto.HapticFeedback.Value;
        if (dto.HighContrastMode.HasValue) prefs.HighContrastMode = dto.HighContrastMode.Value;
        if (dto.PreferredMapApp.HasValue) prefs.PreferredMapApp = dto.PreferredMapApp.Value;
        if (dto.MaxOrdersPerShift.HasValue) prefs.MaxOrdersPerShift = dto.MaxOrdersPerShift.Value;
        if (dto.AutoSendReceipt.HasValue) prefs.AutoSendReceipt = dto.AutoSendReceipt.Value;
        if (dto.LocationTrackingInterval.HasValue) prefs.LocationTrackingInterval = dto.LocationTrackingInterval.Value;
        if (dto.OfflineSyncInterval.HasValue) prefs.OfflineSyncInterval = dto.OfflineSyncInterval.Value;

        prefs.UpdatedAt = DateTime.UtcNow;
        repo.Update(prefs);
        await _unitOfWork.SaveChangesAsync();

        return Result<DriverPreferencesDto>.Success(_mapper.Map<DriverPreferencesDto>(prefs));
    }

    public async Task<Result<bool>> UpdateFocusModeAsync(Guid driverId, UpdateFocusModeDto dto)
    {
        var repo = _unitOfWork.GetRepository<DriverPreferences, Guid>();
        var prefs = (await repo.ListAsync(new DriverPreferencesByDriverSpec(driverId))).FirstOrDefault();
        if (prefs == null) return Result<bool>.NotFound(ErrorMessages.SettingsNotFound);

        prefs.FocusModeAutoTrigger = dto.AutoTrigger;
        prefs.FocusModeSpeedThreshold = dto.SpeedThreshold;
        prefs.UpdatedAt = DateTime.UtcNow;
        repo.Update(prefs);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateQuietHoursAsync(Guid driverId, UpdateQuietHoursDto dto)
    {
        var repo = _unitOfWork.GetRepository<DriverPreferences, Guid>();
        var prefs = (await repo.ListAsync(new DriverPreferencesByDriverSpec(driverId))).FirstOrDefault();
        if (prefs == null) return Result<bool>.NotFound(ErrorMessages.SettingsNotFound);

        prefs.QuietHoursStart = dto.Enabled ? dto.StartTime : null;
        prefs.QuietHoursEnd = dto.Enabled ? dto.EndTime : null;
        prefs.UpdatedAt = DateTime.UtcNow;
        repo.Update(prefs);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateNotificationPrefsAsync(Guid driverId, UpdateNotificationPrefsDto dto)
    {
        var repo = _unitOfWork.GetRepository<DriverPreferences, Guid>();
        var prefs = (await repo.ListAsync(new DriverPreferencesByDriverSpec(driverId))).FirstOrDefault();
        if (prefs == null) return Result<bool>.NotFound(ErrorMessages.SettingsNotFound);

        if (dto.NotifyNewOrder.HasValue) prefs.NotifyNewOrder = dto.NotifyNewOrder.Value;
        if (dto.NotifyCashAlert.HasValue) prefs.NotifyCashAlert = dto.NotifyCashAlert.Value;
        if (dto.NotifyBreakReminder.HasValue) prefs.NotifyBreakReminder = dto.NotifyBreakReminder.Value;
        if (dto.NotifyMaintenance.HasValue) prefs.NotifyMaintenance = dto.NotifyMaintenance.Value;
        if (dto.NotifySettlement.HasValue) prefs.NotifySettlement = dto.NotifySettlement.Value;
        if (dto.NotifyAchievement.HasValue) prefs.NotifyAchievement = dto.NotifyAchievement.Value;
        if (dto.NotifySound.HasValue) prefs.NotifySound = dto.NotifySound.Value;
        if (dto.NotifyVibration.HasValue) prefs.NotifyVibration = dto.NotifyVibration.Value;

        prefs.UpdatedAt = DateTime.UtcNow;
        repo.Update(prefs);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public Task<Result<bool>> UpdateCostParamsAsync(Guid driverId, UpdateCostParamsDto dto)
    {
        // TODO: Implement when CostParams entity is added
        _logger.LogInformation("UpdateCostParams called for driver {DriverId}", driverId);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public async Task<Result<List<NotificationChannelPrefDto>>> GetNotificationChannelsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<NotificationChannelPreference, Guid>();
        var channels = await repo.ListAsync(new NotificationChannelsByDriverSpec(driverId));
        return Result<List<NotificationChannelPrefDto>>.Success(_mapper.Map<List<NotificationChannelPrefDto>>(channels));
    }

    public async Task<Result<NotificationChannelPrefDto>> UpdateNotificationChannelAsync(Guid driverId, NotificationType type, UpdateChannelPrefDto dto)
    {
        var repo = _unitOfWork.GetRepository<NotificationChannelPreference, Guid>();
        var channels = await repo.ListAsync(new NotificationChannelsByDriverSpec(driverId));
        var channel = channels.FirstOrDefault(c => c.NotificationType == type);

        if (channel == null)
            return Result<NotificationChannelPrefDto>.NotFound(ErrorMessages.ChannelSettingsNotFound);

        if (dto.IsEnabled.HasValue) channel.IsEnabled = dto.IsEnabled.Value;
        if (dto.SoundEnabled.HasValue) channel.SoundEnabled = dto.SoundEnabled.Value;
        if (dto.SoundName != null) channel.SoundName = dto.SoundName;
        if (dto.VibrationEnabled.HasValue) channel.VibrationEnabled = dto.VibrationEnabled.Value;
        if (dto.VibrationPattern != null) channel.VibrationPattern = dto.VibrationPattern;
        if (dto.LedColor != null) channel.LedColor = dto.LedColor;
        if (dto.Priority.HasValue) channel.Priority = dto.Priority.Value;
        if (dto.ShowInLockScreen.HasValue) channel.ShowInLockScreen = dto.ShowInLockScreen.Value;
        if (dto.GroupAlerts.HasValue) channel.GroupAlerts = dto.GroupAlerts.Value;

        channel.UpdatedAt = DateTime.UtcNow;
        repo.Update(channel);
        await _unitOfWork.SaveChangesAsync();

        return Result<NotificationChannelPrefDto>.Success(_mapper.Map<NotificationChannelPrefDto>(channel));
    }

    public async Task<Result<List<NotificationChannelPrefDto>>> UpdateNotificationChannelsBulkAsync(Guid driverId, List<UpdateChannelPrefDto> dtos)
    {
        var results = new List<NotificationChannelPrefDto>();
        foreach (var dto in dtos)
        {
            var result = await UpdateNotificationChannelAsync(driverId, dto.NotificationType, dto);
            if (result.IsSuccess)
                results.Add(result.Value!);
        }
        return Result<List<NotificationChannelPrefDto>>.Success(results);
    }

    public async Task<Result<bool>> UpdateHomeLocationAsync(Guid driverId, UpdateHomeLocationDto dto)
    {
        var repo = _unitOfWork.GetRepository<DriverPreferences, Guid>();
        var prefs = (await repo.ListAsync(new DriverPreferencesByDriverSpec(driverId))).FirstOrDefault();
        if (prefs == null) return Result<bool>.NotFound(ErrorMessages.SettingsNotFound);

        prefs.HomeLatitude = dto.HomeLatitude;
        prefs.HomeLongitude = dto.HomeLongitude;
        prefs.HomeAddress = dto.HomeAddress;
        prefs.UpdatedAt = DateTime.UtcNow;
        repo.Update(prefs);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

// Specifications
internal class DriverPreferencesByDriverSpec : Sekka.Core.Specifications.BaseSpecification<DriverPreferences>
{
    public DriverPreferencesByDriverSpec(Guid driverId)
    {
        SetCriteria(p => p.DriverId == driverId);
        AsNoTracking = false;
    }
}

internal class NotificationChannelsByDriverSpec : Sekka.Core.Specifications.BaseSpecification<NotificationChannelPreference>
{
    public NotificationChannelsByDriverSpec(Guid driverId)
    {
        SetCriteria(n => n.DriverId == driverId);
    }
}
