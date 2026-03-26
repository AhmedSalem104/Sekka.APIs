using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface IWhatsAppService
{
    Task<Result<bool>> SendMessageAsync(string phone, string message);
}
