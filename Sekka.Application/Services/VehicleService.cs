using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Vehicle;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<VehicleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<VehicleDto>>> GetVehiclesAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<Vehicle, Guid>();
        var spec = new VehiclesByDriverSpec(driverId);
        var vehicles = await repo.ListAsync(spec);
        var dtos = _mapper.Map<List<VehicleDto>>(vehicles);

        // Compute NextMaintenanceDate from maintenance records
        foreach (var (vehicle, dto) in vehicles.Zip(dtos))
        {
            dto.NextMaintenanceDate = vehicle.MaintenanceRecords
                .Where(m => m.NextDueDate.HasValue)
                .Select(m => m.NextDueDate!.Value)
                .OrderBy(d => d)
                .FirstOrDefault();
        }

        return Result<List<VehicleDto>>.Success(dtos);
    }

    public async Task<Result<VehicleDto>> CreateAsync(Guid driverId, CreateVehicleDto dto)
    {
        var repo = _unitOfWork.GetRepository<Vehicle, Guid>();

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            VehicleType = dto.VehicleType,
            PlateNumber = dto.PlateNumber,
            MakeModel = dto.MakeModel,
            Year = dto.Year,
            CurrentMileageKm = dto.CurrentMileageKm,
            FuelConsumptionPer100Km = dto.FuelConsumptionPer100Km,
            FuelPricePerLiter = dto.FuelPricePerLiter,
            InsuranceExpiryDate = dto.InsuranceExpiryDate,
            IsActive = true,
            ApprovalStatus = VehicleApprovalStatus.Pending
        };

        await repo.AddAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Vehicle {VehicleId} created by driver {DriverId}, type={Type}",
            vehicle.Id, driverId, dto.VehicleType);

        return Result<VehicleDto>.Success(_mapper.Map<VehicleDto>(vehicle));
    }

    public async Task<Result<VehicleDto>> UpdateAsync(Guid driverId, Guid id, UpdateVehicleDto dto)
    {
        var repo = _unitOfWork.GetRepository<Vehicle, Guid>();
        var vehicle = await repo.GetByIdAsync(id);

        if (vehicle == null || vehicle.DriverId != driverId)
            return Result<VehicleDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.VehicleType.HasValue) vehicle.VehicleType = dto.VehicleType.Value;
        if (dto.PlateNumber != null) vehicle.PlateNumber = dto.PlateNumber;
        if (dto.MakeModel != null) vehicle.MakeModel = dto.MakeModel;
        if (dto.Year.HasValue) vehicle.Year = dto.Year.Value;
        if (dto.CurrentMileageKm.HasValue) vehicle.CurrentMileageKm = dto.CurrentMileageKm.Value;
        if (dto.FuelConsumptionPer100Km.HasValue) vehicle.FuelConsumptionPer100Km = dto.FuelConsumptionPer100Km.Value;
        if (dto.FuelPricePerLiter.HasValue) vehicle.FuelPricePerLiter = dto.FuelPricePerLiter.Value;
        if (dto.InsuranceExpiryDate.HasValue) vehicle.InsuranceExpiryDate = dto.InsuranceExpiryDate.Value;

        repo.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return Result<VehicleDto>.Success(_mapper.Map<VehicleDto>(vehicle));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<Vehicle, Guid>();
        var vehicle = await repo.GetByIdAsync(id);

        if (vehicle == null || vehicle.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(vehicle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Vehicle {VehicleId} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<MaintenanceRecordDto>> AddMaintenanceAsync(Guid driverId, Guid vehicleId, CreateMaintenanceDto dto)
    {
        var vehicleRepo = _unitOfWork.GetRepository<Vehicle, Guid>();
        var vehicle = await vehicleRepo.GetByIdAsync(vehicleId);

        if (vehicle == null || vehicle.DriverId != driverId)
            return Result<MaintenanceRecordDto>.NotFound(ErrorMessages.ItemNotFound);

        var maintenanceRepo = _unitOfWork.GetRepository<MaintenanceRecord, Guid>();

        var record = new MaintenanceRecord
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            MaintenanceType = dto.MaintenanceType,
            Cost = dto.Cost,
            MileageAtService = dto.MileageAtService,
            NextDueMileage = dto.NextDueMileage,
            NextDueDate = dto.NextDueDate,
            Notes = dto.Notes,
            ServicedAt = DateTime.UtcNow
        };

        // Update vehicle mileage if maintenance mileage is higher
        if (dto.MileageAtService > vehicle.CurrentMileageKm)
        {
            vehicle.CurrentMileageKm = dto.MileageAtService;
            vehicleRepo.Update(vehicle);
        }

        await maintenanceRepo.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Maintenance {Type} added for vehicle {VehicleId} by driver {DriverId}",
            dto.MaintenanceType, vehicleId, driverId);

        return Result<MaintenanceRecordDto>.Success(_mapper.Map<MaintenanceRecordDto>(record));
    }

    public async Task<Result<List<MaintenanceRecordDto>>> GetMaintenanceAsync(Guid driverId, Guid vehicleId)
    {
        var vehicleRepo = _unitOfWork.GetRepository<Vehicle, Guid>();
        var vehicle = await vehicleRepo.GetByIdAsync(vehicleId);

        if (vehicle == null || vehicle.DriverId != driverId)
            return Result<List<MaintenanceRecordDto>>.NotFound(ErrorMessages.ItemNotFound);

        var maintenanceRepo = _unitOfWork.GetRepository<MaintenanceRecord, Guid>();
        var spec = new MaintenanceByVehicleSpec(vehicleId);
        var records = await maintenanceRepo.ListAsync(spec);

        return Result<List<MaintenanceRecordDto>>.Success(_mapper.Map<List<MaintenanceRecordDto>>(records));
    }
}

// ── Specifications ──

internal class VehiclesByDriverSpec : BaseSpecification<Vehicle>
{
    public VehiclesByDriverSpec(Guid driverId)
    {
        SetCriteria(v => v.DriverId == driverId);
        AddInclude(v => v.MaintenanceRecords);
        SetOrderByDescending(v => v.IsActive);
    }
}

internal class MaintenanceByVehicleSpec : BaseSpecification<MaintenanceRecord>
{
    public MaintenanceByVehicleSpec(Guid vehicleId)
    {
        SetCriteria(m => m.VehicleId == vehicleId);
        SetOrderByDescending(m => m.ServicedAt);
    }
}
