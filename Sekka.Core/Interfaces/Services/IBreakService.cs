using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Vehicle;

namespace Sekka.Core.Interfaces.Services;

public interface IBreakService
{
    Task<Result<BreakLogDto>> StartBreakAsync(Guid driverId, StartBreakDto dto);
    Task<Result<BreakLogDto>> EndBreakAsync(Guid driverId, EndBreakDto dto);
    Task<Result<BreakSuggestionDto>> GetSuggestionAsync(Guid driverId);
    Task<Result<PagedResult<BreakLogDto>>> GetHistoryAsync(Guid driverId, PaginationDto pagination);
}
