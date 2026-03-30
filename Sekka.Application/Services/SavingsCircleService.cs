using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class SavingsCircleService : ISavingsCircleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SavingsCircleService> _logger;

    public SavingsCircleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SavingsCircleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CircleDto>> CreateAsync(Guid driverId, CreateCircleDto dto)
    {
        _logger.LogInformation("CreateCircle by driver {DriverId}", driverId);

        var circleRepo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();

        var circle = new SavingsCircle
        {
            Id = Guid.NewGuid(),
            CreatorDriverId = driverId,
            Name = dto.Name,
            MonthlyAmount = dto.MonthlyAmount,
            MaxMembers = dto.MaxMembers,
            DurationMonths = dto.DurationMonths,
            CurrentRound = 0,
            Status = CircleStatus.Forming,
            MinHealthScore = dto.MinHealthScore
        };

        await circleRepo.AddAsync(circle);

        // Creator is automatically the first member
        var member = new SavingsCircleMember
        {
            Id = Guid.NewGuid(),
            CircleId = circle.Id,
            DriverId = driverId,
            TurnOrder = 1,
            Status = CircleMemberStatus.Active,
            JoinedAt = DateTime.UtcNow
        };
        await memberRepo.AddAsync(member);

        await _unitOfWork.SaveChangesAsync();

        return Result<CircleDto>.Success(new CircleDto
        {
            Id = circle.Id,
            Name = circle.Name,
            MonthlyAmount = circle.MonthlyAmount,
            MaxMembers = circle.MaxMembers,
            CurrentMembersCount = 1,
            DurationMonths = circle.DurationMonths,
            CurrentRound = circle.CurrentRound,
            Status = circle.Status,
            MinHealthScore = circle.MinHealthScore,
            StartDate = circle.StartDate,
            CreatedAt = circle.CreatedAt
        });
    }

    public async Task<Result<PagedResult<CircleDto>>> GetAvailableAsync(PaginationDto pagination)
    {
        _logger.LogInformation("GetAvailableCircles");

        var repo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var spec = new AvailableCirclesSpec();
        var circles = await repo.ListAsync(spec);

        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();

        var dtos = new List<CircleDto>();
        foreach (var c in circles)
        {
            var membersSpec = new CircleMembersSpec(c.Id);
            var members = await memberRepo.ListAsync(membersSpec);

            dtos.Add(new CircleDto
            {
                Id = c.Id,
                Name = c.Name,
                MonthlyAmount = c.MonthlyAmount,
                MaxMembers = c.MaxMembers,
                CurrentMembersCount = members.Count,
                DurationMonths = c.DurationMonths,
                CurrentRound = c.CurrentRound,
                Status = c.Status,
                MinHealthScore = c.MinHealthScore,
                StartDate = c.StartDate,
                CreatedAt = c.CreatedAt
            });
        }

        var paged = dtos
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        return Result<PagedResult<CircleDto>>.Success(
            new PagedResult<CircleDto>(paged, dtos.Count, pagination.Page, pagination.PageSize));
    }

    public async Task<Result<CircleDetailDto>> GetByIdAsync(Guid circleId)
    {
        _logger.LogInformation("GetCircleById {CircleId}", circleId);

        var repo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var circle = await repo.GetByIdAsync(circleId);

        if (circle is null)
            return Result<CircleDetailDto>.NotFound("حلقة التوفير غير موجودة");

        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();
        var membersSpec = new CircleMembersSpec(circleId);
        var members = await memberRepo.ListAsync(membersSpec);

        var paymentRepo = _unitOfWork.GetRepository<SavingsCirclePayment, Guid>();
        var paymentsSpec = new CirclePaymentsSpec(circleId);
        var payments = await paymentRepo.ListAsync(paymentsSpec);

        return Result<CircleDetailDto>.Success(new CircleDetailDto
        {
            Id = circle.Id,
            Name = circle.Name,
            MonthlyAmount = circle.MonthlyAmount,
            MaxMembers = circle.MaxMembers,
            DurationMonths = circle.DurationMonths,
            CurrentRound = circle.CurrentRound,
            Status = circle.Status,
            MinHealthScore = circle.MinHealthScore,
            StartDate = circle.StartDate,
            CreatorDriverId = circle.CreatorDriverId,
            Members = members.Select(m => new CircleMemberDto
            {
                Id = m.Id,
                DriverId = m.DriverId,
                DriverName = string.Empty,
                TurnOrder = m.TurnOrder,
                Status = m.Status,
                JoinedAt = m.JoinedAt
            }).ToList(),
            RecentPayments = payments
                .OrderByDescending(p => p.PaidAt ?? p.CreatedAt)
                .Take(10)
                .Select(p => new CirclePaymentDto
                {
                    Id = p.Id,
                    MemberId = p.MemberId,
                    MemberName = string.Empty,
                    RoundNumber = p.RoundNumber,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaidAt = p.PaidAt
                }).ToList()
        });
    }

    public async Task<Result<List<CircleDto>>> GetMyCirclesAsync(Guid driverId)
    {
        _logger.LogInformation("GetMyCircles for driver {DriverId}", driverId);

        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();
        var myMembershipsSpec = new DriverCircleMembershipsSpec(driverId);
        var memberships = await memberRepo.ListAsync(myMembershipsSpec);

        var circleRepo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var dtos = new List<CircleDto>();

        foreach (var membership in memberships)
        {
            var circle = await circleRepo.GetByIdAsync(membership.CircleId);
            if (circle is null) continue;

            var membersSpec = new CircleMembersSpec(circle.Id);
            var members = await memberRepo.ListAsync(membersSpec);

            dtos.Add(new CircleDto
            {
                Id = circle.Id,
                Name = circle.Name,
                MonthlyAmount = circle.MonthlyAmount,
                MaxMembers = circle.MaxMembers,
                CurrentMembersCount = members.Count,
                DurationMonths = circle.DurationMonths,
                CurrentRound = circle.CurrentRound,
                Status = circle.Status,
                MinHealthScore = circle.MinHealthScore,
                StartDate = circle.StartDate,
                CreatedAt = circle.CreatedAt
            });
        }

        return Result<List<CircleDto>>.Success(dtos);
    }

    public async Task<Result<bool>> JoinAsync(Guid driverId, Guid circleId)
    {
        _logger.LogInformation("JoinCircle {CircleId} by driver {DriverId}", circleId, driverId);

        var circleRepo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var circle = await circleRepo.GetByIdAsync(circleId);

        if (circle is null)
            return Result<bool>.NotFound("حلقة التوفير غير موجودة");

        if (circle.Status != CircleStatus.Forming)
            return Result<bool>.BadRequest("لا يمكن الانضمام - الحلقة ليست في مرحلة التكوين");

        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();
        var membersSpec = new CircleMembersSpec(circleId);
        var members = await memberRepo.ListAsync(membersSpec);

        if (members.Any(m => m.DriverId == driverId && m.Status == CircleMemberStatus.Active))
            return Result<bool>.Conflict("أنت عضو بالفعل في هذه الحلقة");

        if (members.Count(m => m.Status == CircleMemberStatus.Active) >= circle.MaxMembers)
            return Result<bool>.BadRequest("الحلقة ممتلئة");

        var member = new SavingsCircleMember
        {
            Id = Guid.NewGuid(),
            CircleId = circleId,
            DriverId = driverId,
            TurnOrder = members.Count + 1,
            Status = CircleMemberStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        await memberRepo.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> LeaveAsync(Guid driverId, Guid circleId)
    {
        _logger.LogInformation("LeaveCircle {CircleId} by driver {DriverId}", circleId, driverId);

        var circleRepo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var circle = await circleRepo.GetByIdAsync(circleId);

        if (circle is null)
            return Result<bool>.NotFound("حلقة التوفير غير موجودة");

        if (circle.Status != CircleStatus.Forming)
            return Result<bool>.BadRequest("لا يمكن المغادرة بعد بدء الحلقة");

        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();
        var spec = new CircleMemberByDriverSpec(circleId, driverId);
        var members = await memberRepo.ListAsync(spec);
        var member = members.FirstOrDefault();

        if (member is null)
            return Result<bool>.NotFound("أنت لست عضوًا في هذه الحلقة");

        member.Status = CircleMemberStatus.Left;
        memberRepo.Update(member);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<CirclePaymentDto>> MakePaymentAsync(Guid driverId, Guid circleId)
    {
        _logger.LogInformation("MakeCirclePayment for circle {CircleId} by driver {DriverId}", circleId, driverId);

        var circleRepo = _unitOfWork.GetRepository<SavingsCircle, Guid>();
        var circle = await circleRepo.GetByIdAsync(circleId);

        if (circle is null)
            return Result<CirclePaymentDto>.NotFound("حلقة التوفير غير موجودة");

        if (circle.Status != CircleStatus.Active)
            return Result<CirclePaymentDto>.BadRequest("الحلقة غير نشطة");

        var memberRepo = _unitOfWork.GetRepository<SavingsCircleMember, Guid>();
        var memberSpec = new CircleMemberByDriverSpec(circleId, driverId);
        var members = await memberRepo.ListAsync(memberSpec);
        var member = members.FirstOrDefault(m => m.Status == CircleMemberStatus.Active);

        if (member is null)
            return Result<CirclePaymentDto>.NotFound("أنت لست عضوًا نشطًا في هذه الحلقة");

        var paymentRepo = _unitOfWork.GetRepository<SavingsCirclePayment, Guid>();

        var payment = new SavingsCirclePayment
        {
            Id = Guid.NewGuid(),
            CircleId = circleId,
            MemberId = member.Id,
            RoundNumber = circle.CurrentRound,
            Amount = circle.MonthlyAmount,
            Status = CirclePaymentStatus.Paid,
            PaidAt = DateTime.UtcNow
        };

        await paymentRepo.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return Result<CirclePaymentDto>.Success(new CirclePaymentDto
        {
            Id = payment.Id,
            MemberId = payment.MemberId,
            MemberName = string.Empty,
            RoundNumber = payment.RoundNumber,
            Amount = payment.Amount,
            Status = payment.Status,
            PaidAt = payment.PaidAt
        });
    }

    public async Task<Result<List<CirclePaymentDto>>> GetPaymentsAsync(Guid circleId)
    {
        _logger.LogInformation("GetCirclePayments for circle {CircleId}", circleId);

        var paymentRepo = _unitOfWork.GetRepository<SavingsCirclePayment, Guid>();
        var spec = new CirclePaymentsSpec(circleId);
        var payments = await paymentRepo.ListAsync(spec);

        var dtos = payments
            .OrderByDescending(p => p.PaidAt ?? p.CreatedAt)
            .Select(p => new CirclePaymentDto
            {
                Id = p.Id,
                MemberId = p.MemberId,
                MemberName = string.Empty,
                RoundNumber = p.RoundNumber,
                Amount = p.Amount,
                Status = p.Status,
                PaidAt = p.PaidAt
            }).ToList();

        return Result<List<CirclePaymentDto>>.Success(dtos);
    }
}

internal class AvailableCirclesSpec : BaseSpecification<SavingsCircle>
{
    public AvailableCirclesSpec()
    {
        SetCriteria(c => c.Status == CircleStatus.Forming);
        SetOrderByDescending(c => c.CreatedAt);
    }
}

internal class CircleMembersSpec : BaseSpecification<SavingsCircleMember>
{
    public CircleMembersSpec(Guid circleId)
    {
        SetCriteria(m => m.CircleId == circleId && m.Status == CircleMemberStatus.Active);
        SetOrderBy(m => m.TurnOrder);
    }
}

internal class CirclePaymentsSpec : BaseSpecification<SavingsCirclePayment>
{
    public CirclePaymentsSpec(Guid circleId)
    {
        SetCriteria(p => p.CircleId == circleId);
        SetOrderByDescending(p => p.CreatedAt);
    }
}

internal class DriverCircleMembershipsSpec : BaseSpecification<SavingsCircleMember>
{
    public DriverCircleMembershipsSpec(Guid driverId)
    {
        SetCriteria(m => m.DriverId == driverId && m.Status == CircleMemberStatus.Active);
    }
}

internal class CircleMemberByDriverSpec : BaseSpecification<SavingsCircleMember>
{
    public CircleMemberByDriverSpec(Guid circleId, Guid driverId)
    {
        SetCriteria(m => m.CircleId == circleId && m.DriverId == driverId);
    }
}
