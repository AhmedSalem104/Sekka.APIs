using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;

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

    public Task<Result<ReferralCodeDto>> GetMyCodeAsync(Guid driverId)
    {
        _logger.LogWarning("GetMyCode called — feature under development");
        return Task.FromResult(Result<ReferralCodeDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("كود الإحالة")));
    }

    public Task<Result<ReferralStatsDto>> GetStatsAsync(Guid driverId)
    {
        _logger.LogWarning("GetReferralStats called — feature under development");
        return Task.FromResult(Result<ReferralStatsDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("إحصائيات الإحالة")));
    }

    public Task<Result<ReferralDto>> ApplyCodeAsync(Guid driverId, ApplyReferralCodeDto dto)
    {
        _logger.LogWarning("ApplyReferralCode called — feature under development");
        return Task.FromResult(Result<ReferralDto>.BadRequest(ErrorMessages.FeatureUnderDevelopment("تطبيق كود الإحالة")));
    }

    public Task<Result<List<ReferralDto>>> GetMyReferralsAsync(Guid driverId)
    {
        _logger.LogWarning("GetMyReferrals called — feature under development");
        return Task.FromResult(Result<List<ReferralDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("الإحالات")));
    }
}
