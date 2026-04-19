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

    /// <summary>
    /// Get or generate the driver's unique referral code
    /// </summary>
    public async Task<Result<ReferralCodeDto>> GetMyCodeAsync(Guid driverId)
    {
        var code = await GetOrCreateReferralCode(driverId);

        var repo = _unitOfWork.GetRepository<Referral, Guid>();
        var allReferrals = await repo.ListAsync(new ReferralsByReferrerSpec(driverId));

        return Result<ReferralCodeDto>.Success(new ReferralCodeDto
        {
            ReferralCode = code,
            ShareUrl = $"https://sekka.app/join?ref={code}",
            ShareMessage = $"سجّل في سِكّة واستخدم كود الدعوة بتاعي: {code}\n\nhttps://sekka.app/join?ref={code}",
            TotalReferred = allReferrals.Count(r => r.Status >= ReferralStatus.Registered),
            TotalRewards = allReferrals.Count(r => r.ReferrerRewardGiven) * 50m
        });
    }

    /// <summary>
    /// Get referral statistics for the driver
    /// </summary>
    public async Task<Result<ReferralStatsDto>> GetStatsAsync(Guid driverId)
    {
        var code = await GetOrCreateReferralCode(driverId);

        var repo = _unitOfWork.GetRepository<Referral, Guid>();
        var referrals = await repo.ListAsync(new ReferralsByReferrerSpec(driverId));

        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();

        var recentDtos = new List<ReferralDto>();
        foreach (var r in referrals.OrderByDescending(r => r.CreatedAt).Take(10))
        {
            Driver? referred = r.ReferredDriverId.HasValue
                ? await driverRepo.GetByIdAsync(r.ReferredDriverId.Value) : null;

            recentDtos.Add(new ReferralDto
            {
                Id = r.Id,
                ReferredDriverName = referred?.Name,
                ReferredPhone = r.ReferredPhone,
                Status = r.Status,
                StatusText = GetStatusText(r.Status),
                RewardType = r.RewardType,
                RewardGiven = r.RewardGiven,
                CreatedAt = r.CreatedAt,
                RegisteredAt = r.RegisteredAt,
                RewardedAt = r.RewardedAt
            });
        }

        return Result<ReferralStatsDto>.Success(new ReferralStatsDto
        {
            ReferralCode = code,
            ShareUrl = $"https://sekka.app/join?ref={code}",
            TotalReferrals = referrals.Count,
            PendingReferrals = referrals.Count(r => r.Status == ReferralStatus.Pending),
            CompletedReferrals = referrals.Count(r => r.Status == ReferralStatus.Registered),
            RewardedReferrals = referrals.Count(r => r.Status == ReferralStatus.Rewarded),
            TotalPointsEarned = referrals.Count(r => r.ReferrerRewardGiven) * 100,
            TotalCashEarned = referrals.Count(r => r.ReferrerRewardGiven) * 50m,
            RecentReferrals = recentDtos
        });
    }

    /// <summary>
    /// Apply a referral code during registration (called from AuthService)
    /// </summary>
    public async Task<Result<ReferralDto>> ApplyCodeAsync(Guid newDriverId, ApplyReferralCodeDto dto)
    {
        var code = dto.GetCode().Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(code))
            return Result<ReferralDto>.BadRequest("كود الإحالة مطلوب");

        var repo = _unitOfWork.GetRepository<Referral, Guid>();

        // Find the referrer (the person who owns this code)
        var referrerSpec = new ReferralsByCodeOwnerSpec(code);
        var existingReferrals = await repo.ListAsync(referrerSpec);
        var sampleReferral = existingReferrals.FirstOrDefault();

        Guid referrerDriverId;
        if (sampleReferral != null)
        {
            referrerDriverId = sampleReferral.ReferrerDriverId;
        }
        else
        {
            // Code might be driver ID based — try to find driver
            var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
            var allDrivers = await driverRepo.ListAsync(new AllDriversSpec());
            var referrer = allDrivers.FirstOrDefault(d =>
                d.Id.ToString("N")[..8].ToUpperInvariant() == code ||
                d.Id.ToString()[..8].ToUpperInvariant() == code);

            if (referrer == null)
                return Result<ReferralDto>.NotFound("كود الإحالة غير صحيح");

            referrerDriverId = referrer.Id;
        }

        if (referrerDriverId == newDriverId)
            return Result<ReferralDto>.BadRequest("لا يمكنك استخدام كود الإحالة الخاص بك");

        // Check if already referred
        var existingForDriver = await repo.ListAsync(new ReferralByReferredDriverSpec(newDriverId));
        if (existingForDriver.Any())
            return Result<ReferralDto>.BadRequest("لقد استخدمت كود إحالة بالفعل");

        // Create the referral record
        var newDriver = await _unitOfWork.GetRepository<Driver, Guid>().GetByIdAsync(newDriverId);

        var referral = new Referral
        {
            ReferrerDriverId = referrerDriverId,
            ReferredDriverId = newDriverId,
            ReferralCode = code,
            ReferredPhone = newDriver?.PhoneNumber,
            Status = ReferralStatus.Registered,
            RewardType = RewardType.Points,
            RegisteredAt = DateTime.UtcNow
        };

        await repo.AddAsync(referral);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Referral applied: {NewDriverId} used code {Code} from {ReferrerId}",
            newDriverId, code, referrerDriverId);

        return Result<ReferralDto>.Success(new ReferralDto
        {
            Id = referral.Id,
            ReferredDriverName = newDriver?.Name,
            ReferredPhone = referral.ReferredPhone,
            Status = referral.Status,
            StatusText = GetStatusText(referral.Status),
            RewardType = referral.RewardType,
            RewardGiven = false,
            CreatedAt = referral.CreatedAt,
            RegisteredAt = referral.RegisteredAt
        });
    }

    /// <summary>
    /// Get all referrals for a driver
    /// </summary>
    public async Task<Result<List<ReferralDto>>> GetMyReferralsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Referral, Guid>();
        var referrals = await repo.ListAsync(new ReferralsByReferrerSpec(driverId));
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();

        var dtos = new List<ReferralDto>();
        foreach (var r in referrals.OrderByDescending(r => r.CreatedAt))
        {
            Driver? referred = r.ReferredDriverId.HasValue
                ? await driverRepo.GetByIdAsync(r.ReferredDriverId.Value) : null;

            dtos.Add(new ReferralDto
            {
                Id = r.Id,
                ReferredDriverName = referred?.Name,
                ReferredPhone = r.ReferredPhone,
                Status = r.Status,
                StatusText = GetStatusText(r.Status),
                RewardType = r.RewardType,
                RewardGiven = r.RewardGiven,
                CreatedAt = r.CreatedAt,
                RegisteredAt = r.RegisteredAt,
                RewardedAt = r.RewardedAt
            });
        }

        return Result<List<ReferralDto>>.Success(dtos);
    }

    // ── Helpers ──

    private async Task<string> GetOrCreateReferralCode(Guid driverId)
    {
        // Generate deterministic code from driver ID
        return driverId.ToString("N")[..8].ToUpperInvariant();
    }

    private static string GetStatusText(ReferralStatus status) => status switch
    {
        ReferralStatus.Pending => "في الانتظار",
        ReferralStatus.Registered => "تم التسجيل",
        ReferralStatus.Rewarded => "تم المكافأة",
        ReferralStatus.Expired => "منتهي",
        _ => "غير معروف"
    };
}

// ── Specifications ──

internal class ReferralsByReferrerSpec : BaseSpecification<Referral>
{
    public ReferralsByReferrerSpec(Guid driverId)
    {
        SetCriteria(r => r.ReferrerDriverId == driverId);
        SetOrderByDescending(r => r.CreatedAt);
    }
}

internal class ReferralsByCodeOwnerSpec : BaseSpecification<Referral>
{
    public ReferralsByCodeOwnerSpec(string code)
    {
        SetCriteria(r => r.ReferralCode == code);
    }
}

internal class ReferralByReferredDriverSpec : BaseSpecification<Referral>
{
    public ReferralByReferredDriverSpec(Guid driverId)
    {
        SetCriteria(r => r.ReferredDriverId == driverId);
    }
}

internal class AllDriversSpec : BaseSpecification<Driver>
{
    public AllDriversSpec()
    {
        SetCriteria(d => d.IsActive);
    }
}
