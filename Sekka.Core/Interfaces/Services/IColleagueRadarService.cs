using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface IColleagueRadarService
{
    Task<Result<List<NearbyDriverDto>>> GetNearbyAsync(Guid driverId, double latitude, double longitude, double radiusKm);
    Task<Result<HelpRequestDto>> CreateHelpRequestAsync(Guid driverId, CreateHelpRequestDto dto);
    Task<Result<List<HelpRequestDto>>> GetNearbyHelpRequestsAsync(Guid driverId, double latitude, double longitude, double radiusKm);
    Task<Result<HelpRequestDto>> RespondToHelpRequestAsync(Guid driverId, Guid requestId);
    Task<Result<bool>> ResolveHelpRequestAsync(Guid driverId, Guid requestId);
    Task<Result<List<HelpRequestDto>>> GetMyHelpRequestsAsync(Guid driverId);
    Task<Result<bool>> UpdateLocationAsync(Guid driverId, UpdateLocationDto dto);
}
