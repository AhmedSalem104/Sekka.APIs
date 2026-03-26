using Sekka.Core.Common;
using Sekka.Core.DTOs.Customer;

namespace Sekka.Core.Interfaces.Services;

public interface ICallerIdService
{
    Task<Result<CallerIdDto>> LookupAsync(Guid driverId, string phone);
    Task<Result<CallerIdNoteDto>> CreateAsync(Guid driverId, CreateCallerIdNoteDto dto);
    Task<Result<CallerIdNoteDto>> UpdateAsync(Guid driverId, Guid id, UpdateCallerIdNoteDto dto);
    Task<Result<bool>> DeleteAsync(Guid driverId, Guid id);
    Task<Result<TruecallerLookupDto>> TruecallerLookupAsync(string phone);
}
