using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.DTOs.Partner;

namespace Sekka.Core.Interfaces.Services;

public interface IPartnerService
{
    Task<Result<List<PartnerDto>>> GetAllAsync(Guid driverId);
    Task<Result<PartnerDto>> CreateAsync(Guid driverId, CreatePartnerDto dto);
    Task<Result<PartnerDto>> UpdateAsync(Guid driverId, Guid id, UpdatePartnerDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid driverId, Guid partnerId, PaginationDto pagination);
    Task<Result<List<PickupPointDto>>> GetPickupPointsAsync(Guid driverId, Guid partnerId);
    Task<Result<PartnerDto>> SubmitVerificationAsync(Guid driverId, Guid id, Stream fileStream, string fileName);
    Task<Result<PartnerVerificationDto>> GetVerificationStatusAsync(Guid driverId, Guid id);
}
