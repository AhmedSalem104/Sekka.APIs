using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Communication;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class MessageTemplateService : IMessageTemplateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MessageTemplateService> _logger;

    public MessageTemplateService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MessageTemplateService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<MessageTemplateDto>>> GetTemplatesAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<QuickMessageTemplate, Guid>();
        var spec = new TemplatesByDriverSpec(driverId);
        var items = await repo.ListAsync(spec);

        var dtos = _mapper.Map<List<MessageTemplateDto>>(items);
        return Result<List<MessageTemplateDto>>.Success(dtos);
    }

    public async Task<Result<MessageTemplateDto>> CreateAsync(Guid driverId, CreateMessageTemplateDto dto)
    {
        var repo = _unitOfWork.GetRepository<QuickMessageTemplate, Guid>();

        var template = new QuickMessageTemplate
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            MessageText = dto.MessageText,
            Category = dto.Category,
            IsSystemTemplate = false,
            UsageCount = 0,
            SortOrder = 0
        };

        await repo.AddAsync(template);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Message template {TemplateId} created by driver {DriverId}", template.Id, driverId);

        return Result<MessageTemplateDto>.Success(_mapper.Map<MessageTemplateDto>(template));
    }

    public async Task<Result<MessageTemplateDto>> UpdateAsync(Guid driverId, Guid id, UpdateMessageTemplateDto dto)
    {
        var repo = _unitOfWork.GetRepository<QuickMessageTemplate, Guid>();
        var template = await repo.GetByIdAsync(id);

        if (template == null || template.DriverId != driverId)
            return Result<MessageTemplateDto>.NotFound(ErrorMessages.MessageTemplateNotFound);

        if (template.IsSystemTemplate)
            return Result<MessageTemplateDto>.BadRequest(ErrorMessages.CannotDeleteSystemTemplate);

        if (dto.MessageText != null) template.MessageText = dto.MessageText;
        if (dto.Category.HasValue) template.Category = dto.Category.Value;
        if (dto.SortOrder.HasValue) template.SortOrder = dto.SortOrder.Value;

        repo.Update(template);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Message template {TemplateId} updated by driver {DriverId}", id, driverId);

        return Result<MessageTemplateDto>.Success(_mapper.Map<MessageTemplateDto>(template));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<QuickMessageTemplate, Guid>();
        var template = await repo.GetByIdAsync(id);

        if (template == null || template.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.MessageTemplateNotFound);

        if (template.IsSystemTemplate)
            return Result<bool>.BadRequest(ErrorMessages.CannotDeleteSystemTemplate);

        repo.Delete(template);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Message template {TemplateId} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RecordUsageAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<QuickMessageTemplate, Guid>();
        var template = await repo.GetByIdAsync(id);

        if (template == null || (template.DriverId != null && template.DriverId != driverId))
            return Result<bool>.NotFound(ErrorMessages.MessageTemplateNotFound);

        template.UsageCount++;
        repo.Update(template);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

// ── Specifications ──

internal class TemplatesByDriverSpec : BaseSpecification<QuickMessageTemplate>
{
    public TemplatesByDriverSpec(Guid driverId)
    {
        // Return driver's own templates + system templates
        SetCriteria(t => t.DriverId == driverId || t.IsSystemTemplate);
        SetOrderBy(t => t.SortOrder);
    }
}
