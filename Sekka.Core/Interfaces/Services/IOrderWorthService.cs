using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IOrderWorthService
{
    Task<Result<OrderWorthDto>> CalculateWorthAsync(Guid driverId, Guid orderId);
    Task<Result<PriceCalculationResultDto>> CalculatePriceAsync(PriceCalculationRequestDto dto);
}
