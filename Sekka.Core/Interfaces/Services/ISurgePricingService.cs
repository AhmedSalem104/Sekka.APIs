using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface ISurgePricingService
{
    Task<Result<SurgeMultiplierDto>> CalculateMultiplierAsync(Guid regionId, DateTime dateTime);
}
