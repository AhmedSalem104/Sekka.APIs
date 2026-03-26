using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface IBulkImportService
{
    Task<Result<BulkImportResultDto>> ImportAsync(Guid driverId, BulkImportDto dto);
}
