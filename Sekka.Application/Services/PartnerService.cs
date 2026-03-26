using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class PartnerService : IPartnerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PartnerService> _logger;

    public PartnerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PartnerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<PartnerDto>>> GetAllAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Partner, Guid>();
        var spec = new PartnersByDriverSpec(driverId);
        var partners = await repo.ListAsync(spec);

        return Result<List<PartnerDto>>.Success(_mapper.Map<List<PartnerDto>>(partners));
    }

    public async Task<Result<PartnerDto>> CreateAsync(Guid driverId, CreatePartnerDto dto)
    {
        var repo = _unitOfWork.GetRepository<Partner, Guid>();

        var partner = _mapper.Map<Partner>(dto);
        partner.Id = Guid.NewGuid();
        partner.DriverId = driverId;
        partner.Color = dto.Color ?? "#3B82F6";
        partner.VerificationStatus = VerificationStatus.Unverified;

        await repo.AddAsync(partner);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Partner {PartnerName} created by driver {DriverId}", dto.Name, driverId);

        return Result<PartnerDto>.Success(_mapper.Map<PartnerDto>(partner));
    }

    public async Task<Result<PartnerDto>> UpdateAsync(Guid driverId, Guid id, UpdatePartnerDto dto)
    {
        var repo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await repo.GetByIdAsync(id);

        if (partner == null || partner.DriverId != driverId)
            return Result<PartnerDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.Name != null) partner.Name = dto.Name;
        if (dto.Phone != null) partner.Phone = dto.Phone;
        if (dto.Address != null) partner.Address = dto.Address;
        if (dto.CommissionType.HasValue) partner.CommissionType = dto.CommissionType.Value;
        if (dto.CommissionValue.HasValue) partner.CommissionValue = dto.CommissionValue.Value;
        if (dto.Color != null) partner.Color = dto.Color;
        if (dto.LogoUrl != null) partner.LogoUrl = dto.LogoUrl;
        if (dto.ReceiptHeader != null) partner.ReceiptHeader = dto.ReceiptHeader;
        if (dto.IsActive.HasValue) partner.IsActive = dto.IsActive.Value;

        repo.Update(partner);
        await _unitOfWork.SaveChangesAsync();

        return Result<PartnerDto>.Success(_mapper.Map<PartnerDto>(partner));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await repo.GetByIdAsync(id);

        if (partner == null || partner.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(partner);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Partner {Id} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid driverId, Guid partnerId, PaginationDto pagination)
    {
        // TODO: Query orders by partnerId using specifications
        return Task.FromResult(Result<PagedResult<OrderListDto>>.Success(
            new PagedResult<OrderListDto>(new List<OrderListDto>(), 0, pagination.Page, pagination.PageSize)));
    }

    public async Task<Result<List<PickupPointDto>>> GetPickupPointsAsync(Guid driverId, Guid partnerId)
    {
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(partnerId);

        if (partner == null || partner.DriverId != driverId)
            return Result<List<PickupPointDto>>.NotFound(ErrorMessages.ItemNotFound);

        var pickupRepo = _unitOfWork.GetRepository<PickupPoint, Guid>();
        var spec = new PickupPointsByPartnerSpec(partnerId);
        var points = await pickupRepo.ListAsync(spec);

        return Result<List<PickupPointDto>>.Success(_mapper.Map<List<PickupPointDto>>(points));
    }

    public async Task<Result<PartnerDto>> SubmitVerificationAsync(Guid driverId, Guid id, Stream fileStream, string fileName)
    {
        var repo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await repo.GetByIdAsync(id);

        if (partner == null || partner.DriverId != driverId)
            return Result<PartnerDto>.NotFound(ErrorMessages.ItemNotFound);

        // TODO: Upload file to storage
        var documentUrl = $"/uploads/partners/{id}/verification{Path.GetExtension(fileName)}";
        partner.VerificationDocumentUrl = documentUrl;
        partner.VerificationStatus = VerificationStatus.Pending;

        repo.Update(partner);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Partner {Id} verification submitted by driver {DriverId}", id, driverId);

        return Result<PartnerDto>.Success(_mapper.Map<PartnerDto>(partner));
    }

    public async Task<Result<PartnerVerificationDto>> GetVerificationStatusAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await repo.GetByIdAsync(id);

        if (partner == null || partner.DriverId != driverId)
            return Result<PartnerVerificationDto>.NotFound(ErrorMessages.ItemNotFound);

        return Result<PartnerVerificationDto>.Success(new PartnerVerificationDto
        {
            Status = partner.VerificationStatus,
            DocumentUrl = partner.VerificationDocumentUrl,
            Note = partner.VerificationNote,
            VerifiedAt = partner.VerifiedAt
        });
    }
}

// ── Specifications ──

internal class PartnersByDriverSpec : BaseSpecification<Partner>
{
    public PartnersByDriverSpec(Guid driverId)
    {
        SetCriteria(p => p.DriverId == driverId);
        SetOrderBy(p => p.Name);
    }
}

internal class PickupPointsByPartnerSpec : BaseSpecification<PickupPoint>
{
    public PickupPointsByPartnerSpec(Guid partnerId)
    {
        SetCriteria(p => p.PartnerId == partnerId && p.IsActive);
        SetOrderBy(p => p.Name);
    }
}
