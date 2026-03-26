using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(ILogger<WhatsAppService> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> SendMessageAsync(string phone, string message)
    {
        _logger.LogWarning("WhatsAppService.SendMessageAsync called but feature is under development");
        return Task.FromResult(Result<bool>.BadRequest(
            ErrorMessages.FeatureUnderDevelopment("رسائل واتساب")));
    }
}
