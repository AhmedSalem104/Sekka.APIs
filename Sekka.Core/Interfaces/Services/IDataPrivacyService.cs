using Sekka.Core.Common;
using Sekka.Core.DTOs.Privacy;

namespace Sekka.Core.Interfaces.Services;

public interface IDataPrivacyService
{
    Task<Result<List<ConsentDto>>> GetConsentsAsync(Guid driverId);
    Task<Result<ConsentDto>> UpdateConsentAsync(Guid driverId, string consentType, UpdateConsentDto dto);
    Task<Result<DataExportDto>> RequestDataExportAsync(Guid driverId);
    Task<Result<DeletionRequestDto>> RequestDataDeletionAsync(Guid driverId, DeletionRequestDto dto);
    Task<Result<DeletionRequestDto>> GetDeletionStatusAsync(Guid driverId);
}
