using Sekka.Core.Common;
using Sekka.Core.DTOs.Social;

namespace Sekka.Core.Interfaces.Services;

public interface IShiftService
{
    Task<Result<ShiftDto>> StartShiftAsync(Guid driverId, StartShiftDto dto);
    Task<Result<ShiftDto>> EndShiftAsync(Guid driverId);
    Task<Result<ShiftDto>> GetCurrentShiftAsync(Guid driverId);
    Task<Result<ShiftSummaryDto>> GetSummaryAsync(Guid driverId, DateOnly? from, DateOnly? to);
}
