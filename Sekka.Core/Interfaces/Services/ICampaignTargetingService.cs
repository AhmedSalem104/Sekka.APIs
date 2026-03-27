using Sekka.Core.Common;
using Sekka.Core.DTOs.Admin;
using Sekka.Core.DTOs.Common;

namespace Sekka.Core.Interfaces.Services;

public interface ICampaignTargetingService
{
    Task<Result<PagedResult<AdminCampaignDto>>> GetCampaignsAsync(CampaignFilterDto filter);
    Task<Result<AdminCampaignDetailDto>> CreateAsync(CreateCampaignDto dto);
    Task<Result<AdminCampaignDetailDto>> UpdateAsync(Guid id, UpdateCampaignDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<bool>> LaunchAsync(Guid id);
    Task<Result<bool>> PauseAsync(Guid id);
    Task<Result<bool>> ResumeAsync(Guid id);
    Task<Result<CampaignStatsDto>> GetStatsAsync();
}
