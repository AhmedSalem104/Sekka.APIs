using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.System;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class WebhookService : IWebhookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WebhookService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<WebhookConfigDto>>> GetAllAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<WebhookConfig, Guid>();
        var spec = new WebhooksByDriverSpec(driverId);
        var webhooks = await repo.ListAsync(spec);

        var dtos = webhooks.Select(w => new WebhookConfigDto
        {
            Id = w.Id,
            Name = w.Name,
            Url = w.Url,
            PartnerName = w.Partner?.Name,
            Events = DeserializeEvents(w.Events),
            IsActive = w.IsActive,
            LastTriggeredAt = w.LastTriggeredAt,
            FailureCount = w.FailureCount
        }).ToList();

        return Result<List<WebhookConfigDto>>.Success(dtos);
    }

    public async Task<Result<WebhookConfigDto>> CreateAsync(Guid driverId, CreateWebhookConfigDto dto)
    {
        var repo = _unitOfWork.GetRepository<WebhookConfig, Guid>();

        var webhook = new WebhookConfig
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            PartnerId = dto.PartnerId,
            Name = dto.Name,
            Url = dto.Url,
            Secret = Guid.NewGuid().ToString("N"),
            Events = JsonSerializer.Serialize(dto.Events),
            IsActive = true,
            FailureCount = 0
        };

        await repo.AddAsync(webhook);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Webhook {WebhookId} created by driver {DriverId}", webhook.Id, driverId);

        return Result<WebhookConfigDto>.Success(new WebhookConfigDto
        {
            Id = webhook.Id,
            Name = webhook.Name,
            Url = webhook.Url,
            Events = dto.Events,
            IsActive = webhook.IsActive,
            FailureCount = 0
        });
    }

    public async Task<Result<WebhookConfigDto>> UpdateAsync(Guid driverId, Guid id, UpdateWebhookConfigDto dto)
    {
        var repo = _unitOfWork.GetRepository<WebhookConfig, Guid>();
        var webhook = await repo.GetByIdAsync(id);

        if (webhook == null || webhook.DriverId != driverId)
            return Result<WebhookConfigDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.Name != null) webhook.Name = dto.Name;
        if (dto.Url != null) webhook.Url = dto.Url;
        if (dto.Events != null) webhook.Events = JsonSerializer.Serialize(dto.Events);
        if (dto.IsActive.HasValue) webhook.IsActive = dto.IsActive.Value;

        repo.Update(webhook);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Webhook {WebhookId} updated by driver {DriverId}", id, driverId);

        return Result<WebhookConfigDto>.Success(new WebhookConfigDto
        {
            Id = webhook.Id,
            Name = webhook.Name,
            Url = webhook.Url,
            PartnerName = webhook.Partner?.Name,
            Events = DeserializeEvents(webhook.Events),
            IsActive = webhook.IsActive,
            LastTriggeredAt = webhook.LastTriggeredAt,
            FailureCount = webhook.FailureCount
        });
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<WebhookConfig, Guid>();
        var webhook = await repo.GetByIdAsync(id);

        if (webhook == null || webhook.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(webhook);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Webhook {WebhookId} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResult<WebhookLogDto>>> GetLogsAsync(Guid driverId, Guid id, PaginationDto pagination)
    {
        var configRepo = _unitOfWork.GetRepository<WebhookConfig, Guid>();
        var webhook = await configRepo.GetByIdAsync(id);

        if (webhook == null || webhook.DriverId != driverId)
            return Result<PagedResult<WebhookLogDto>>.NotFound(ErrorMessages.ItemNotFound);

        var logRepo = _unitOfWork.GetRepository<WebhookLog, Guid>();
        var spec = new WebhookLogsByConfigSpec(id, pagination);
        var logs = await logRepo.ListAsync(spec);
        var countSpec = new WebhookLogsByConfigCountSpec(id);
        var total = await logRepo.CountAsync(countSpec);

        var dtos = logs.Select(l => new WebhookLogDto
        {
            Id = l.Id,
            EventType = l.EventType,
            IsSuccess = l.IsSuccess,
            ResponseStatusCode = l.ResponseStatusCode,
            RetryCount = l.RetryCount,
            SentAt = l.SentAt
        }).ToList();

        return Result<PagedResult<WebhookLogDto>>.Success(
            new PagedResult<WebhookLogDto>(dtos, total, pagination.Page, pagination.PageSize));
    }

    public async Task<Result<bool>> TestAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<WebhookConfig, Guid>();
        var webhook = await repo.GetByIdAsync(id);

        if (webhook == null || webhook.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        // TODO: Send HTTP POST to webhook.Url with test payload
        _logger.LogInformation("Webhook {WebhookId} test triggered by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    private static List<string> DeserializeEvents(string json)
    {
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? new(); }
        catch { return new(); }
    }
}

// ── Specifications ──

internal class WebhooksByDriverSpec : BaseSpecification<WebhookConfig>
{
    public WebhooksByDriverSpec(Guid driverId)
    {
        SetCriteria(w => w.DriverId == driverId);
        AddInclude(w => w.Partner!);
        SetOrderByDescending(w => w.Id);
    }
}

internal class WebhookLogsByConfigSpec : BaseSpecification<WebhookLog>
{
    public WebhookLogsByConfigSpec(Guid webhookConfigId, PaginationDto pagination)
    {
        SetCriteria(l => l.WebhookConfigId == webhookConfigId);
        SetOrderByDescending(l => l.SentAt);
        ApplyPaging((pagination.Page - 1) * pagination.PageSize, pagination.PageSize);
    }
}

internal class WebhookLogsByConfigCountSpec : BaseSpecification<WebhookLog>
{
    public WebhookLogsByConfigCountSpec(Guid webhookConfigId)
    {
        SetCriteria(l => l.WebhookConfigId == webhookConfigId);
    }
}
