using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;

namespace Sekka.Core.Interfaces.Services;

public interface ITimeSlotService
{
    Task<Result<AvailableSlotsDto>> GetAvailableSlotsAsync(DateOnly date, Guid? regionId);
    Task<Result<OrderDto>> BookSlotAsync(Guid driverId, Guid orderId, BookSlotDto dto);
    Task<Result<List<TimeSlotDto>>> GetTimeSlotsAsync(DateOnly date, Guid? regionId);
    Task<Result<TimeSlotDto>> CreateTimeSlotAsync(CreateTimeSlotDto dto);
    Task<Result<TimeSlotDto>> UpdateTimeSlotAsync(Guid id, UpdateTimeSlotDto dto);
    Task<Result<bool>> DeleteTimeSlotAsync(Guid id);
    Task<Result<List<TimeSlotDto>>> GenerateWeekSlotsAsync(GenerateWeekSlotsDto dto);
    Task<Result<TimeSlotStatsDto>> GetStatsAsync(DateOnly? dateFrom, DateOnly? dateTo);
}
