using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class ReferralService : IReferralService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ReferralService> _logger;

    public ReferralService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReferralService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ReferralCodeDto>> GetMyCodeAsync(Guid driverId)
    {
        // The referral code is derived from the driver's ID (first 8 chars uppercase)
        var repo = _unitOfWork.GetRepository<Referral, Guid>();
        var spec = new ReferralsByReferrerSpec(driverId);
        var referrals = await repo.ListAsync(spec);

        // Use existing code from first referral or generate one
        var code = referrals.FirstOrDefault()?.ReferralCode
            ?? $"SEK-{driverId.ToString("N")[..8].ToUpperInvariant()}";

        var dto = new ReferralCodeDto
        {
            ReferralCode = code,
            ShareUrl = $"https://sekka.app/join?ref={code}"
        };

        return Result<ReferralCodeDto>.Success(dto);
    }

    public async Task<Result<ReferralStatsDto>> GetStatsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Referral, Guid>();
        var spec = new ReferralsByReferrerSpec(driverId);
        var referrals = await repo.ListAsync(spec);

        var code = referrals.FirstOrDefault()?.ReferralCode
            ?? $"SEK-{driverId.ToString("N")[..8].ToUpperInvariant()}";

        var stats = new ReferralStatsDto
        {
            ReferralCode = code,
            TotalReferrals = referrals.Count,
            CompletedReferrals = referrals.Count(r => r.Status == ReferralStatus.Rewarded),
            PendingReferrals = referrals.Count(r => r.Status == ReferralStatus.Pending),
            TotalPointsEarned = referrals.Count(r => r.RewardGiven) * 50 // 50 points per completed referral
        };

        return Result<ReferralStatsDto>.Success(stats);
    }

    public async Task<Result<ReferralDto>> ApplyCodeAsync(Guid driverId, ApplyReferralCodeDto dto)
    {
        var repo = _unitOfWork.GetRepository<Referral, Guid>();

        // Find the referral entry by code
        var spec = new ReferralByCodeSpec(dto.ReferralCode);
        var referrals = await repo.ListAsync(spec);
        var referral = referrals.FirstOrDefault();

        if (referral is null)
            return Result<ReferralDto>.NotFound("كود الإحالة غير موجود");

        if (referral.ReferrerDriverId == driverId)
            return Result<ReferralDto>.BadRequest("لا يمكنك استخدام كود الإحالة الخاص بك");

        if (referral.ReferredDriverId.HasValue)
            return Result<ReferralDto>.BadRequest("كود الإحالة مستخدم بالفعل");

        // Link the referred driver
        referral.ReferredDriverId = driverId;
        referral.Status = ReferralStatus.Registered;
        referral.RegisteredAt = DateTime.UtcNow;

        repo.Update(referral);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Driver {DriverId} applied referral code {Code}", driverId, dto.ReferralCode);

        return Result<ReferralDto>.Success(_mapper.Map<ReferralDto>(referral));
    }

    public async Task<Result<List<ReferralDto>>> GetMyReferralsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Referral, Guid>();
        var spec = new ReferralsByReferrerSpec(driverId);
        var referrals = await repo.ListAsync(spec);

        var dtos = _mapper.Map<List<ReferralDto>>(referrals);
        return Result<List<ReferralDto>>.Success(dtos);
    }
}

// ── Specifications ──

internal class ReferralsByReferrerSpec : BaseSpecification<Referral>
{
    public ReferralsByReferrerSpec(Guid driverId)
    {
        SetCriteria(r => r.ReferrerDriverId == driverId);
    }
}

internal class ReferralByCodeSpec : BaseSpecification<Referral>
{
    public ReferralByCodeSpec(string code)
    {
        SetCriteria(r => r.ReferralCode == code);
    }
}
