using Sekka.Core.Common;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.System;

namespace Sekka.Core.Interfaces.Services;

public interface IAuditLogService
{
    Task<Result<PagedResult<AuditLogDto>>> GetLogsAsync(AuditLogFilterDto filter);
    Task<Result<List<AuditLogDto>>> GetEntityLogsAsync(string entityType, string entityId);
    Task<Result<PagedResult<AuditLogDto>>> GetUserLogsAsync(Guid userId, PaginationDto pagination);
    Task<Result<AuditActionsSummaryDto>> GetSummaryAsync(DateTime? dateFrom, DateTime? dateTo);
}
