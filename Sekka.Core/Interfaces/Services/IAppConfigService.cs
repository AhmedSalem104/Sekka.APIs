using Sekka.Core.Common;
using Sekka.Core.DTOs.System;
using Sekka.Core.Enums;

namespace Sekka.Core.Interfaces.Services;

public interface IAppConfigService
{
    Task<Result<AppVersionCheckDto>> CheckVersionAsync(AppPlatform platform, string currentVersion);
    Task<Result<List<SystemNoticeDto>>> GetNoticesAsync(Guid? driverId);
    Task<Result<FeatureFlagsCheckDto>> GetFeaturesAsync(Guid? driverId);
}
