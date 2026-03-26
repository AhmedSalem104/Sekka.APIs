using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Communication;

namespace Sekka.Core.Interfaces.Services;

public interface ISOSService
{
    Task<Result<SOSLogDto>> ActivateAsync(Guid driverId, ActivateSOSDto dto);
    Task<Result<bool>> DismissAsync(Guid driverId, Guid id);
    Task<Result<SOSLogDto>> ResolveAsync(Guid driverId, Guid id, ResolveSOSDto dto);
    Task<Result<PagedResult<SOSLogDto>>> GetHistoryAsync(Guid driverId, PaginationDto pagination);
}
