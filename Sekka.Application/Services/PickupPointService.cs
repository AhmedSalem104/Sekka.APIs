using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Partner;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class PickupPointService : IPickupPointService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PickupPointService> _logger;

    public PickupPointService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PickupPointService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PickupPointDto>> CreateAsync(Guid driverId, CreatePickupPointDto dto)
    {
        // Verify partner ownership
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(dto.PartnerId);

        if (partner == null || partner.DriverId != driverId)
            return Result<PickupPointDto>.NotFound(ErrorMessages.ItemNotFound);

        var repo = _unitOfWork.GetRepository<PickupPoint, Guid>();

        var point = new PickupPoint
        {
            Id = Guid.NewGuid(),
            PartnerId = dto.PartnerId,
            Name = dto.Name,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Notes = dto.Notes,
            IsActive = true
        };

        await repo.AddAsync(point);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Pickup point {Name} created for partner {PartnerId}", dto.Name, dto.PartnerId);

        var result = _mapper.Map<PickupPointDto>(point);
        result.PartnerName = partner.Name;
        return Result<PickupPointDto>.Success(result);
    }

    public async Task<Result<PickupPointDto>> UpdateAsync(Guid driverId, Guid id, UpdatePickupPointDto dto)
    {
        var repo = _unitOfWork.GetRepository<PickupPoint, Guid>();
        var point = await repo.GetByIdAsync(id);

        if (point == null)
            return Result<PickupPointDto>.NotFound(ErrorMessages.ItemNotFound);

        // Verify partner ownership
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(point.PartnerId);

        if (partner == null || partner.DriverId != driverId)
            return Result<PickupPointDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.Name != null) point.Name = dto.Name;
        if (dto.Address != null) point.Address = dto.Address;
        if (dto.Latitude.HasValue) point.Latitude = dto.Latitude;
        if (dto.Longitude.HasValue) point.Longitude = dto.Longitude;
        if (dto.Notes != null) point.Notes = dto.Notes;
        if (dto.IsActive.HasValue) point.IsActive = dto.IsActive.Value;

        repo.Update(point);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<PickupPointDto>(point);
        result.PartnerName = partner.Name;
        return Result<PickupPointDto>.Success(result);
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<PickupPoint, Guid>();
        var point = await repo.GetByIdAsync(id);

        if (point == null)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        // Verify partner ownership
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(point.PartnerId);

        if (partner == null || partner.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(point);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Pickup point {Id} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<PickupPointDto>> RateAsync(Guid driverId, Guid id, RatePickupPointDto dto)
    {
        var repo = _unitOfWork.GetRepository<PickupPoint, Guid>();
        var point = await repo.GetByIdAsync(id);

        if (point == null)
            return Result<PickupPointDto>.NotFound(ErrorMessages.ItemNotFound);

        // Verify partner ownership
        var partnerRepo = _unitOfWork.GetRepository<Partner, Guid>();
        var partner = await partnerRepo.GetByIdAsync(point.PartnerId);

        if (partner == null || partner.DriverId != driverId)
            return Result<PickupPointDto>.NotFound(ErrorMessages.ItemNotFound);

        // Update average rating (weighted average)
        var totalVisits = point.VisitCount + 1;
        point.DriverRating = ((point.DriverRating * point.VisitCount) + dto.Rating) / totalVisits;
        point.VisitCount = totalVisits;

        if (dto.WaitingMinutes.HasValue)
        {
            point.AverageWaitingMinutes = ((point.AverageWaitingMinutes * (totalVisits - 1)) + dto.WaitingMinutes.Value) / totalVisits;
        }

        repo.Update(point);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Pickup point {Id} rated {Rating} by driver {DriverId}", id, dto.Rating, driverId);

        var result = _mapper.Map<PickupPointDto>(point);
        result.PartnerName = partner.Name;
        return Result<PickupPointDto>.Success(result);
    }
}
