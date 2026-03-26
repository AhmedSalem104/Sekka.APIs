using Sekka.Core.Common;
using Sekka.Core.DTOs.Vehicle;

namespace Sekka.Core.Interfaces.Services;

public interface IVehicleService
{
    Task<Result<List<VehicleDto>>> GetVehiclesAsync(Guid driverId);
    Task<Result<VehicleDto>> CreateAsync(Guid driverId, CreateVehicleDto dto);
    Task<Result<VehicleDto>> UpdateAsync(Guid driverId, Guid id, UpdateVehicleDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<MaintenanceRecordDto>> AddMaintenanceAsync(Guid driverId, Guid vehicleId, CreateMaintenanceDto dto);
    Task<Result<List<MaintenanceRecordDto>>> GetMaintenanceAsync(Guid driverId, Guid vehicleId);
}
