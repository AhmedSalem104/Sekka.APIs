using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface IEmailService
{
    Task<Result<bool>> SendAsync(string to, string subject, string htmlBody);
    Task<Result<bool>> SendTemplateAsync(string to, string templateName, Dictionary<string, string> parameters);
}
