using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IAddressSwapService
{
    Task<Result<AddressSwapLogDto>> SwapAddressAsync(Guid driverId, Guid orderId, SwapAddressDto dto);
}
