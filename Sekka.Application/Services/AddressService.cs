using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddressService> _logger;

    public AddressService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddressService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<AddressDto>>> SearchAsync(Guid driverId, AddressSearchDto search)
    {
        var repo = _unitOfWork.GetRepository<Address, Guid>();
        var spec = new AddressSearchSpec(driverId, search);
        var addresses = await repo.ListAsync(spec);

        return Result<List<AddressDto>>.Success(_mapper.Map<List<AddressDto>>(addresses));
    }

    public async Task<Result<AddressDto>> SaveAsync(Guid driverId, SaveAddressDto dto)
    {
        var repo = _unitOfWork.GetRepository<Address, Guid>();

        var address = new Address
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            CustomerId = dto.CustomerId,
            AddressText = dto.AddressText,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            AddressType = dto.AddressType,
            Landmarks = dto.Landmarks,
            DeliveryNotes = dto.DeliveryNotes,
            VisitCount = 1,
            LastVisitedAt = DateTime.UtcNow
        };

        await repo.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Address saved for driver {DriverId}: {AddressText}", driverId, dto.AddressText);

        return Result<AddressDto>.Success(_mapper.Map<AddressDto>(address));
    }

    public async Task<Result<AddressDto>> UpdateAsync(Guid driverId, Guid id, UpdateAddressDto dto)
    {
        var repo = _unitOfWork.GetRepository<Address, Guid>();
        var address = await repo.GetByIdAsync(id);

        if (address == null || address.DriverId != driverId)
            return Result<AddressDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.AddressText != null) address.AddressText = dto.AddressText;
        if (dto.Latitude.HasValue) address.Latitude = dto.Latitude;
        if (dto.Longitude.HasValue) address.Longitude = dto.Longitude;
        if (dto.Landmarks != null) address.Landmarks = dto.Landmarks;
        if (dto.DeliveryNotes != null) address.DeliveryNotes = dto.DeliveryNotes;

        repo.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return Result<AddressDto>.Success(_mapper.Map<AddressDto>(address));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<Address, Guid>();
        var address = await repo.GetByIdAsync(id);

        if (address == null || address.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(address);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Address {Id} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<AddressDto>>> AutocompleteAsync(Guid driverId, string q, double? latitude, double? longitude)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Result<List<AddressDto>>.Success(new List<AddressDto>());

        var repo = _unitOfWork.GetRepository<Address, Guid>();
        var spec = new AddressAutocompleteSpec(driverId, q);
        var addresses = await repo.ListAsync(spec);

        return Result<List<AddressDto>>.Success(_mapper.Map<List<AddressDto>>(addresses));
    }

    public async Task<Result<List<AddressDto>>> NearbyAsync(Guid driverId, double latitude, double longitude, double radiusKm)
    {
        // Fetch all driver addresses and filter in-memory using Haversine
        // (for DB-level spatial queries, PostGIS or similar would be needed)
        var repo = _unitOfWork.GetRepository<Address, Guid>();
        var spec = new AddressesWithCoordinatesSpec(driverId);
        var allAddresses = await repo.ListAsync(spec);

        var nearby = allAddresses
            .Where(a => a.Latitude.HasValue && a.Longitude.HasValue)
            .Select(a => new { Address = a, Distance = HaversineDistance(latitude, longitude, a.Latitude!.Value, a.Longitude!.Value) })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Take(20)
            .Select(x => x.Address)
            .ToList();

        return Result<List<AddressDto>>.Success(_mapper.Map<List<AddressDto>>(nearby));
    }

    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}

// ── Specifications ──

internal class AddressAutocompleteSpec : BaseSpecification<Address>
{
    public AddressAutocompleteSpec(Guid driverId, string query)
    {
        var q = query.ToLowerInvariant();
        SetCriteria(a => a.DriverId == driverId && a.AddressText.ToLower().Contains(q));
        SetOrderByDescending(a => a.VisitCount);
        ApplyPaging(0, 10);
    }
}

internal class AddressesWithCoordinatesSpec : BaseSpecification<Address>
{
    public AddressesWithCoordinatesSpec(Guid driverId)
    {
        SetCriteria(a => a.DriverId == driverId && a.Latitude != null && a.Longitude != null);
    }
}

internal class AddressSearchSpec : BaseSpecification<Address>
{
    public AddressSearchSpec(Guid driverId, AddressSearchDto search)
    {
        if (search.CustomerId.HasValue && search.AddressType.HasValue)
            SetCriteria(a => a.DriverId == driverId && a.CustomerId == search.CustomerId.Value && a.AddressType == search.AddressType.Value);
        else if (search.CustomerId.HasValue)
            SetCriteria(a => a.DriverId == driverId && a.CustomerId == search.CustomerId.Value);
        else if (search.AddressType.HasValue)
            SetCriteria(a => a.DriverId == driverId && a.AddressType == search.AddressType.Value);
        else
            SetCriteria(a => a.DriverId == driverId);

        SetOrderByDescending(a => a.LastVisitedAt!);
        ApplyPaging((search.Page - 1) * search.PageSize, search.PageSize);
    }
}
