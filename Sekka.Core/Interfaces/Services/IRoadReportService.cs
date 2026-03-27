using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface IRoadReportService
{
    Task<Result<RoadReportDto>> CreateAsync(Guid driverId, CreateRoadReportDto dto);
    Task<Result<List<RoadReportDto>>> GetNearbyAsync(double latitude, double longitude, double radiusKm);
    Task<Result<bool>> ConfirmAsync(Guid driverId, Guid reportId, bool isConfirmed);
    Task<Result<RoadReportDto>> GetByIdAsync(Guid reportId);
    Task<Result<List<RoadReportDto>>> GetMyReportsAsync(Guid driverId);
    Task<Result<bool>> DeactivateAsync(Guid driverId, Guid reportId);
}
