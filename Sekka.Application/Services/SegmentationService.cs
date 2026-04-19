using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.DTOs.Common;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class SegmentationService : ISegmentationService
{
    private readonly ILogger<SegmentationService> _logger;

    public SegmentationService(ILogger<SegmentationService> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> RefreshSegmentAsync(Guid segmentId)
        => Task.FromResult(Result<bool>.NotImplemented(ErrorMessages.FeatureUnderDevelopment("تحديث الشريحة")));

    public Task<Result<PagedResult<SegmentMemberDto>>> GetSegmentMembersAsync(Guid segmentId, PaginationDto pagination)
        => Task.FromResult(Result<PagedResult<SegmentMemberDto>>.NotImplemented(ErrorMessages.FeatureUnderDevelopment("أعضاء الشريحة")));
}
