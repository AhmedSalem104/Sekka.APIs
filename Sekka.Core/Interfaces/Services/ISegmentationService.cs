using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.DTOs.Common;

namespace Sekka.Core.Interfaces.Services;

public interface ISegmentationService
{
    Task<Result<bool>> RefreshSegmentAsync(Guid segmentId);
    Task<Result<PagedResult<SegmentMemberDto>>> GetSegmentMembersAsync(Guid segmentId, PaginationDto pagination);
}
