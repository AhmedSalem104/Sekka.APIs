using Sekka.Core.Common;
using Sekka.Core.DTOs.Account;

namespace Sekka.Core.Interfaces.Services;

public interface IAccountManagementService
{
    Task<Result<bool>> RequestAccountDeletionAsync(Guid driverId, DeleteAccountDto dto);
    Task<Result<bool>> ConfirmAccountDeletionAsync(Guid driverId, ConfirmDeletionDto dto);
    Task<Result<List<ActiveSessionDto>>> GetActiveSessionsAsync(Guid driverId);
    Task<Result<bool>> TerminateSessionAsync(Guid driverId, Guid sessionId);
    Task<Result<bool>> LogoutAllDevicesAsync(Guid driverId);
}
