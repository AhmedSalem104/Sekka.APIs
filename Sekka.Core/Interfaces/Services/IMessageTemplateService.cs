using Sekka.Core.Common;
using Sekka.Core.DTOs.Communication;

namespace Sekka.Core.Interfaces.Services;

public interface IMessageTemplateService
{
    Task<Result<List<MessageTemplateDto>>> GetTemplatesAsync(Guid driverId);
    Task<Result<MessageTemplateDto>> CreateAsync(Guid driverId, CreateMessageTemplateDto dto);
    Task<Result<MessageTemplateDto>> UpdateAsync(Guid driverId, Guid id, UpdateMessageTemplateDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<bool>> RecordUsageAsync(Guid driverId, Guid id);
}
