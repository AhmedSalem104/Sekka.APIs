using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IRefundService
{
    Task<Result<RefundDto>> CreateAsync(Guid driverId, CreateRefundDto dto);
    Task<Result<List<RefundDto>>> GetRefundsAsync(Guid orderId);
}
