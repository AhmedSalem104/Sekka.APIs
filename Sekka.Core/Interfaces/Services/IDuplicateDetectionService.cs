using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IDuplicateDetectionService
{
    Task<Result<DuplicateResultDto>> CheckDuplicateAsync(Guid driverId, CheckDuplicateDto dto);
}
