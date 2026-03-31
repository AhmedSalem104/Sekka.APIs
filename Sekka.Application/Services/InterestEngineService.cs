using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Intelligence;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class InterestEngineService : IInterestEngineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InterestEngineService> _logger;

    public InterestEngineService(IUnitOfWork unitOfWork, ILogger<InterestEngineService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> RecordSignalAsync(Guid driverId, Guid customerId, string signalType, string? categoryId, string? metadata)
    {
        // TODO: Create InterestSignal entity and persist to DB
        _logger.LogInformation("Recording interest signal: Driver={DriverId}, Customer={CustomerId}, Type={SignalType}",
            driverId, customerId, signalType);

        // Stub: signal recorded successfully
        return await Task.FromResult(Result<bool>.Success(true));
    }

    public async Task<Result<CustomerInterestProfileDto>> GetCustomerProfileAsync(Guid driverId, Guid customerId)
    {
        // TODO: Build profile from real DB data (interests, segments, behavior, RFM)
        _logger.LogInformation("Getting customer profile: Driver={DriverId}, Customer={CustomerId}", driverId, customerId);

        var profile = new CustomerInterestProfileDto
        {
            CustomerId = customerId,
            CustomerPhone = "01000000000",
            EngagementLevel = "Medium",
            LifetimeValue = 0,
            TotalOrders = 0,
            TopInterests = new List<CustomerInterestDto>(),
            CurrentSegments = new List<SegmentBriefDto>(),
            RfmScore = new RfmScoreDto
            {
                RecencyScore = 0,
                FrequencyScore = 0,
                MonetaryScore = 0,
                TotalScore = 0,
                Segment = "New"
            }
        };

        return await Task.FromResult(Result<CustomerInterestProfileDto>.Success(profile));
    }

    public async Task<Result<List<CustomerInterestDto>>> GetCustomerInterestsAsync(Guid driverId, Guid customerId)
    {
        // TODO: Read from InterestScore table
        _logger.LogInformation("Getting customer interests: Driver={DriverId}, Customer={CustomerId}", driverId, customerId);
        return await Task.FromResult(Result<List<CustomerInterestDto>>.Success(new List<CustomerInterestDto>()));
    }

    public async Task<Result<List<InterestCategorySummaryDto>>> GetTopInterestsAsync(Guid driverId, TopInterestsQueryDto query)
    {
        var catRepo = _unitOfWork.GetRepository<InterestCategory, Guid>();
        var categories = await catRepo.ListAsync(new ActiveCategoriesSpec());

        var interestRepo = _unitOfWork.GetRepository<CustomerInterest, Guid>();
        var interests = await interestRepo.ListAsync(new InterestsByDriverSpec(driverId));

        var result = categories.Select(cat =>
        {
            var catInterests = interests.Where(i => i.CategoryId == cat.Id).ToList();
            return new InterestCategorySummaryDto
            {
                CategoryId = cat.Id,
                CategoryName = cat.Name,
                CategoryNameAr = cat.NameAr,
                CategoryColor = cat.ColorHex,
                CustomerCount = catInterests.Select(i => i.CustomerId).Distinct().Count(),
                TotalOrders = catInterests.Sum(i => i.SignalCount),
                AverageScore = catInterests.Any() ? Math.Round(catInterests.Average(i => i.Score), 2) : 0
            };
        })
        .OrderByDescending(c => c.CustomerCount)
        .Take(query.Limit > 0 ? query.Limit : 10)
        .ToList();

        return Result<List<InterestCategorySummaryDto>>.Success(result);
    }

    public async Task<Result<List<CustomerSegmentSummaryDto>>> GetSegmentsAsync(Guid driverId)
    {
        var segRepo = _unitOfWork.GetRepository<CustomerSegment, Guid>();
        var segments = await segRepo.ListAsync(new ActiveSegmentsSpec());

        var memberRepo = _unitOfWork.GetRepository<CustomerSegmentMember, Guid>();
        var members = await memberRepo.ListAsync(new MembersByDriverSpec(driverId));

        var totalMembers = members.Select(m => m.CustomerId).Distinct().Count();

        var result = segments.Select(s =>
        {
            var segMembers = members.Count(m => m.SegmentId == s.Id && m.IsActive);
            return new CustomerSegmentSummaryDto
            {
                SegmentId = s.Id,
                Name = s.Name,
                NameAr = s.NameAr,
                ColorHex = s.ColorHex,
                MemberCount = segMembers,
                PercentageOfTotal = totalMembers > 0 ? Math.Round((decimal)segMembers / totalMembers * 100, 1) : 0
            };
        })
        .OrderByDescending(s => s.MemberCount)
        .ToList();

        return Result<List<CustomerSegmentSummaryDto>>.Success(result);
    }

    public async Task<Result<PagedResult<CustomerInterestProfileDto>>> GetSegmentCustomersAsync(Guid driverId, Guid segmentId, PaginationDto pagination)
    {
        var memberRepo = _unitOfWork.GetRepository<CustomerSegmentMember, Guid>();
        var members = await memberRepo.ListAsync(new SegmentMembersPagedSpec(segmentId, driverId));

        var paged = members
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(m => new CustomerInterestProfileDto
            {
                CustomerId = m.CustomerId,
                CustomerPhone = "",
                TotalOrders = 0,
                TopInterests = new List<CustomerInterestDto>(),
                CurrentSegments = new List<SegmentBriefDto>(),
                RfmScore = new RfmScoreDto()
            })
            .ToList();

        return Result<PagedResult<CustomerInterestProfileDto>>.Success(
            new PagedResult<CustomerInterestProfileDto>(paged, members.Count, pagination.Page, pagination.PageSize));
    }
}

internal class ActiveCategoriesSpec : BaseSpecification<InterestCategory>
{
    public ActiveCategoriesSpec() { SetCriteria(c => c.IsActive); SetOrderBy(c => c.SortOrder); }
}

internal class InterestsByDriverSpec : BaseSpecification<CustomerInterest>
{
    public InterestsByDriverSpec(Guid driverId) { SetCriteria(i => i.DriverId == driverId); }
}

internal class ActiveSegmentsSpec : BaseSpecification<CustomerSegment>
{
    public ActiveSegmentsSpec() { SetCriteria(s => s.IsActive); }
}

internal class MembersByDriverSpec : BaseSpecification<CustomerSegmentMember>
{
    public MembersByDriverSpec(Guid driverId) { SetCriteria(m => m.DriverId == driverId && m.IsActive); }
}

internal class SegmentMembersPagedSpec : BaseSpecification<CustomerSegmentMember>
{
    public SegmentMembersPagedSpec(Guid segmentId, Guid driverId)
    {
        SetCriteria(m => m.SegmentId == segmentId && m.DriverId == driverId && m.IsActive);
    }
}
