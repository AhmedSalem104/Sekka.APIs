using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IDisputeService
{
    Task<Result<DisputeDto>> CreateAsync(Guid driverId, CreateDisputeDto dto);
    Task<Result<List<DisputeDto>>> GetDisputesAsync(Guid orderId);
}
