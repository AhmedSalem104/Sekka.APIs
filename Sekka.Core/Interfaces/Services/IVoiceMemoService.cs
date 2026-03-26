using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IVoiceMemoService
{
    Task<Result<VoiceMemoDto>> UploadMemoAsync(Guid driverId, Guid? orderId, Guid? customerId, Stream audioStream, string fileName);
    Task<Result<List<VoiceMemoDto>>> GetMemosAsync(Guid driverId, Guid? orderId, Guid? customerId);
    Task<Result<bool>> DeleteMemoAsync(Guid driverId, Guid memoId);
}
