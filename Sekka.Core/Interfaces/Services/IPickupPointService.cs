using Sekka.Core.Common;
using Sekka.Core.DTOs.Partner;

namespace Sekka.Core.Interfaces.Services;

public interface IPickupPointService
{
    Task<Result<PickupPointDto>> CreateAsync(Guid driverId, CreatePickupPointDto dto);
    Task<Result<PickupPointDto>> UpdateAsync(Guid driverId, Guid id, UpdatePickupPointDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<PickupPointDto>> RateAsync(Guid driverId, Guid id, RatePickupPointDto dto);
}
