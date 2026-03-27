using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.System;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(ILogger<AuditLogService> logger)
    {
        _logger = logger;
    }

    public Task<Result<PagedResult<AuditLogDto>>> GetLogsAsync(AuditLogFilterDto filter)
    {
        return Task.FromResult(Result<PagedResult<AuditLogDto>>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("سجل المراجعة")));
    }

    public Task<Result<List<AuditLogDto>>> GetEntityLogsAsync(string entityType, string entityId)
    {
        return Task.FromResult(Result<List<AuditLogDto>>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("سجل مراجعة الكيان")));
    }

    public Task<Result<PagedResult<AuditLogDto>>> GetUserLogsAsync(Guid userId, PaginationDto pagination)
    {
        return Task.FromResult(Result<PagedResult<AuditLogDto>>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("سجل مراجعة المستخدم")));
    }

    public Task<Result<AuditActionsSummaryDto>> GetSummaryAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        return Task.FromResult(Result<AuditActionsSummaryDto>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("ملخص المراجعة")));
    }
}
