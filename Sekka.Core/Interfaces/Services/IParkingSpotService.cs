using Sekka.Core.Common;
using Sekka.Core.DTOs.Vehicle;

namespace Sekka.Core.Interfaces.Services;

public interface IParkingSpotService
{
    Task<Result<List<ParkingSpotDto>>> GetAllAsync(Guid driverId);
    Task<Result<ParkingSpotDto>> CreateAsync(Guid driverId, CreateParkingSpotDto dto);
    Task<Result<ParkingSpotDto>> UpdateAsync(Guid driverId, Guid id, UpdateParkingSpotDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<List<ParkingSpotDto>>> GetNearbyAsync(Guid driverId, NearbyQueryDto query);
}
