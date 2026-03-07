using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public Task<Result<bool>> SendAsync(string to, string subject, string htmlBody)
    {
        // TODO: Implement SMTP/SendGrid email sending
        _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
        return Task.FromResult(Result<bool>.Success(true));
    }

    public Task<Result<bool>> SendTemplateAsync(string to, string templateName, Dictionary<string, string> parameters)
    {
        // TODO: Implement template-based email
        _logger.LogInformation("Template email sent to {To}: {Template}", to, templateName);
        return Task.FromResult(Result<bool>.Success(true));
    }
}
