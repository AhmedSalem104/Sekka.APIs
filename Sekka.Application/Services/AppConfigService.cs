using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.System;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class AppConfigService : IAppConfigService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AppConfigService> _logger;

    public AppConfigService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AppConfigService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AppVersionCheckDto>> CheckVersionAsync(AppPlatform platform, string currentVersion)
    {
        var repo = _unitOfWork.GetRepository<AppVersion, Guid>();
        var spec = new LatestAppVersionSpec(platform);
        var versions = await repo.ListAsync(spec);
        var latest = versions.FirstOrDefault();

        if (latest == null)
            return Result<AppVersionCheckDto>.NotFound("لا يوجد إصدار متاح لهذه المنصة");

        var dto = new AppVersionCheckDto
        {
            CurrentVersion = currentVersion,
            LatestVersion = latest.VersionCode,
            MinRequiredVersion = latest.MinRequiredVersion,
            IsForceUpdate = latest.IsForceUpdate,
            StoreUrl = latest.StoreUrl,
            ReleaseNotes = latest.ReleaseNotes
        };

        return Result<AppVersionCheckDto>.Success(dto);
    }

    public async Task<Result<List<SystemNoticeDto>>> GetNoticesAsync(Guid? driverId)
    {
        var repo = _unitOfWork.GetRepository<SystemNotice, Guid>();
        var spec = new ActiveSystemNoticesSpec();
        var notices = await repo.ListAsync(spec);

        var dtos = notices.Select(n => new SystemNoticeDto
        {
            Id = n.Id,
            Title = n.Title,
            TitleEn = n.TitleEn,
            Body = n.Body,
            BodyEn = n.BodyEn,
            NoticeType = n.NoticeType,
            TargetAudience = n.TargetAudience,
            ActionUrl = n.ActionUrl,
            ActionLabel = n.ActionLabel,
            BackgroundColor = n.BackgroundColor,
            Priority = n.Priority,
            StartsAt = n.StartsAt,
            ExpiresAt = n.ExpiresAt,
            IsDismissable = n.IsDismissable,
            IsActive = n.IsActive,
            ViewCount = n.ViewCount,
            ClickCount = n.ClickCount
        }).ToList();

        return Result<List<SystemNoticeDto>>.Success(dtos);
    }

    public Task<Result<FeatureFlagsCheckDto>> GetFeaturesAsync(Guid? driverId)
    {
        // TODO: Query feature flags and evaluate per-driver eligibility
        return Task.FromResult(Result<FeatureFlagsCheckDto>.Success(
            new FeatureFlagsCheckDto { Features = new Dictionary<string, bool>() }));
    }
}

// ── Specifications ──

internal class LatestAppVersionSpec : BaseSpecification<AppVersion>
{
    public LatestAppVersionSpec(AppPlatform platform)
    {
        SetCriteria(v => v.Platform == platform && v.IsActive);
        SetOrderByDescending(v => v.VersionNumber);
        ApplyPaging(0, 1);
    }
}

internal class ActiveSystemNoticesSpec : BaseSpecification<SystemNotice>
{
    public ActiveSystemNoticesSpec()
    {
        var now = DateTime.UtcNow;
        SetCriteria(n => n.IsActive && n.StartsAt <= now && (n.ExpiresAt == null || n.ExpiresAt > now));
        SetOrderByDescending(n => n.Priority);
    }
}
