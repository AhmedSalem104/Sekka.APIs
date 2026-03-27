using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.System;

namespace Sekka.Core.Interfaces.Services;

public interface IWebhookService
{
    Task<Result<List<WebhookConfigDto>>> GetAllAsync(Guid driverId);
    Task<Result<WebhookConfigDto>> CreateAsync(Guid driverId, CreateWebhookConfigDto dto);
    Task<Result<WebhookConfigDto>> UpdateAsync(Guid driverId, Guid id, UpdateWebhookConfigDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<PagedResult<WebhookLogDto>>> GetLogsAsync(Guid driverId, Guid id, PaginationDto pagination);
    Task<Result<bool>> TestAsync(Guid driverId, Guid id);
}
