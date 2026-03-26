using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Vehicle;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class ParkingSpotService : IParkingSpotService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ParkingSpotService> _logger;

    public ParkingSpotService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ParkingSpotService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ParkingSpotDto>>> GetAllAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<ParkingSpot, Guid>();
        var spec = new ParkingSpotsByDriverSpec(driverId);
        var spots = await repo.ListAsync(spec);
        var dtos = _mapper.Map<List<ParkingSpotDto>>(spots);
        return Result<List<ParkingSpotDto>>.Success(dtos);
    }

    public async Task<Result<ParkingSpotDto>> CreateAsync(Guid driverId, CreateParkingSpotDto dto)
    {
        var repo = _unitOfWork.GetRepository<ParkingSpot, Guid>();

        var spot = new ParkingSpot
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = dto.Address,
            Notes = dto.Notes,
            QualityRating = dto.QualityRating,
            IsPaid = dto.IsPaid,
            PaidAmount = dto.PaidAmount,
            IsShared = dto.IsShared,
            UsageCount = 1,
            LastUsedAt = DateTime.UtcNow
        };

        await repo.AddAsync(spot);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Parking spot {SpotId} created by driver {DriverId}", spot.Id, driverId);

        return Result<ParkingSpotDto>.Success(_mapper.Map<ParkingSpotDto>(spot));
    }

    public async Task<Result<ParkingSpotDto>> UpdateAsync(Guid driverId, Guid id, UpdateParkingSpotDto dto)
    {
        var repo = _unitOfWork.GetRepository<ParkingSpot, Guid>();
        var spot = await repo.GetByIdAsync(id);

        if (spot == null || spot.DriverId != driverId)
            return Result<ParkingSpotDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.Latitude.HasValue) spot.Latitude = dto.Latitude.Value;
        if (dto.Longitude.HasValue) spot.Longitude = dto.Longitude.Value;
        if (dto.Address != null) spot.Address = dto.Address;
        if (dto.Notes != null) spot.Notes = dto.Notes;
        if (dto.QualityRating.HasValue) spot.QualityRating = dto.QualityRating.Value;
        if (dto.IsPaid.HasValue) spot.IsPaid = dto.IsPaid.Value;
        if (dto.PaidAmount.HasValue) spot.PaidAmount = dto.PaidAmount.Value;
        if (dto.IsShared.HasValue) spot.IsShared = dto.IsShared.Value;

        spot.LastUsedAt = DateTime.UtcNow;
        spot.UsageCount++;

        repo.Update(spot);
        await _unitOfWork.SaveChangesAsync();

        return Result<ParkingSpotDto>.Success(_mapper.Map<ParkingSpotDto>(spot));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<ParkingSpot, Guid>();
        var spot = await repo.GetByIdAsync(id);

        if (spot == null || spot.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(spot);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Parking spot {SpotId} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public Task<Result<List<ParkingSpotDto>>> GetNearbyAsync(Guid driverId, NearbyQueryDto query)
    {
        // TODO: Implement spatial query for nearby shared parking spots
        // For now return empty list — requires geospatial index or Haversine formula
        _logger.LogInformation("Nearby parking query by driver {DriverId}: lat={Lat}, lng={Lng}, radius={Radius}km",
            driverId, query.Latitude, query.Longitude, query.RadiusKm);

        return Task.FromResult(Result<List<ParkingSpotDto>>.Success(new List<ParkingSpotDto>()));
    }
}

// ── Specifications ──

internal class ParkingSpotsByDriverSpec : BaseSpecification<ParkingSpot>
{
    public ParkingSpotsByDriverSpec(Guid driverId)
    {
        SetCriteria(p => p.DriverId == driverId);
        SetOrderByDescending(p => p.LastUsedAt);
    }
}
