using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.HealthScore;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class HealthScoreService : IHealthScoreService
{
    private readonly ILogger<HealthScoreService> _logger;

    public HealthScoreService(ILogger<HealthScoreService> logger)
    {
        _logger = logger;
    }

    public Task<Result<AccountHealthDto>> GetHealthScoreAsync(Guid driverId)
    {
        // TODO: Calculate from real data (orders, ratings, settlements)
        return Task.FromResult(Result<AccountHealthDto>.Success(new AccountHealthDto
        {
            OverallScore = 85,
            SuccessRateScore = 90,
            CustomerRatingScore = 80,
            CommitmentScore = 85,
            ActivityScore = 80,
            CashHandlingScore = 90,
            Status = "Good",
            LastCalculatedAt = DateTime.UtcNow,
            Trend = "stable"
        }));
    }

    public Task<Result<List<HealthTipDto>>> GetHealthTipsAsync(Guid driverId)
    {
        // TODO: Generate tips based on actual health score
        return Task.FromResult(Result<List<HealthTipDto>>.Success(new List<HealthTipDto>()));
    }
}
