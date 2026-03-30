using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TimeSlotService> _logger;

    public TimeSlotService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TimeSlotService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AvailableSlotsDto>> GetAvailableSlotsAsync(DateOnly date, Guid? regionId)
    {
        _logger.LogInformation("Available slots requested for date {Date}, region {RegionId}", date, regionId);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var spec = new TimeSlotsByDateSpec(date, regionId);
        var slots = await repo.ListAsync(spec);

        var dto = new AvailableSlotsDto
        {
            Date = date,
            Slots = slots
                .Where(s => s.CurrentBookings < s.MaxCapacity)
                .Select(MapToDto)
                .ToList()
        };

        return Result<AvailableSlotsDto>.Success(dto);
    }

    public async Task<Result<OrderDto>> BookSlotAsync(Guid driverId, Guid orderId, BookSlotDto dto)
    {
        _logger.LogInformation("Book slot {SlotId} requested by driver {DriverId} for order {OrderId}", dto.TimeSlotId, driverId, orderId);

        var slotRepo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var slot = await slotRepo.GetByIdAsync(dto.TimeSlotId);

        if (slot is null)
            return Result<OrderDto>.NotFound("الفترة الزمنية غير موجودة");

        if (!slot.IsActive)
            return Result<OrderDto>.BadRequest("الفترة الزمنية غير متاحة");

        if (slot.CurrentBookings >= slot.MaxCapacity)
            return Result<OrderDto>.BadRequest("الفترة الزمنية ممتلئة");

        slot.CurrentBookings++;
        slotRepo.Update(slot);

        // Update order time window
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order is null || order.DriverId != driverId)
            return Result<OrderDto>.NotFound("الطلب غير موجود");

        order.TimeWindowStart = slot.Date.ToDateTime(slot.StartTime);
        order.TimeWindowEnd = slot.Date.ToDateTime(slot.EndTime);
        order.ScheduledDate = slot.Date;
        orderRepo.Update(order);

        await _unitOfWork.SaveChangesAsync();

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<Result<List<TimeSlotDto>>> GetTimeSlotsAsync(DateOnly date, Guid? regionId)
    {
        _logger.LogInformation("Time slots requested for date {Date}, region {RegionId}", date, regionId);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var spec = new TimeSlotsByDateSpec(date, regionId);
        var slots = await repo.ListAsync(spec);

        return Result<List<TimeSlotDto>>.Success(slots.Select(MapToDto).ToList());
    }

    public async Task<Result<TimeSlotDto>> CreateTimeSlotAsync(CreateTimeSlotDto dto)
    {
        _logger.LogInformation("Create time slot for date {Date}", dto.Date);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();

        var slot = new DeliveryTimeSlot
        {
            Id = Guid.NewGuid(),
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            MaxCapacity = dto.MaxCapacity,
            CurrentBookings = 0,
            RegionId = dto.RegionId,
            PriceMultiplier = dto.PriceMultiplier,
            IsActive = true
        };

        await repo.AddAsync(slot);
        await _unitOfWork.SaveChangesAsync();

        return Result<TimeSlotDto>.Success(MapToDto(slot));
    }

    public async Task<Result<TimeSlotDto>> UpdateTimeSlotAsync(Guid id, UpdateTimeSlotDto dto)
    {
        _logger.LogInformation("Update time slot {SlotId}", id);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var slot = await repo.GetByIdAsync(id);

        if (slot is null)
            return Result<TimeSlotDto>.NotFound("الفترة الزمنية غير موجودة");

        if (dto.MaxCapacity.HasValue) slot.MaxCapacity = dto.MaxCapacity.Value;
        if (dto.PriceMultiplier.HasValue) slot.PriceMultiplier = dto.PriceMultiplier.Value;
        if (dto.IsActive.HasValue) slot.IsActive = dto.IsActive.Value;

        repo.Update(slot);
        await _unitOfWork.SaveChangesAsync();

        return Result<TimeSlotDto>.Success(MapToDto(slot));
    }

    public async Task<Result<bool>> DeleteTimeSlotAsync(Guid id)
    {
        _logger.LogInformation("Delete time slot {SlotId}", id);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var slot = await repo.GetByIdAsync(id);

        if (slot is null)
            return Result<bool>.NotFound("الفترة الزمنية غير موجودة");

        if (slot.CurrentBookings > 0)
            return Result<bool>.BadRequest("لا يمكن حذف فترة زمنية بها حجوزات");

        repo.Delete(slot);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<TimeSlotDto>>> GenerateWeekSlotsAsync(GenerateWeekSlotsDto dto)
    {
        _logger.LogInformation("Generate week slots starting {StartDate}", dto.StartDate);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var generatedSlots = new List<DeliveryTimeSlot>();

        for (var day = 0; day < 7; day++)
        {
            var date = dto.StartDate.AddDays(day);
            var currentTime = dto.DayStart;

            while (currentTime.AddMinutes(dto.SlotDurationMinutes) <= dto.DayEnd)
            {
                var endTime = currentTime.AddMinutes(dto.SlotDurationMinutes);

                var slot = new DeliveryTimeSlot
                {
                    Id = Guid.NewGuid(),
                    Date = date,
                    StartTime = currentTime,
                    EndTime = endTime,
                    MaxCapacity = dto.MaxCapacityPerSlot,
                    CurrentBookings = 0,
                    RegionId = dto.RegionId,
                    PriceMultiplier = 1.0m,
                    IsActive = true
                };

                await repo.AddAsync(slot);
                generatedSlots.Add(slot);
                currentTime = endTime;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<List<TimeSlotDto>>.Success(generatedSlots.Select(MapToDto).ToList());
    }

    public async Task<Result<TimeSlotStatsDto>> GetStatsAsync(DateOnly? dateFrom, DateOnly? dateTo)
    {
        _logger.LogInformation("Time slot stats from {DateFrom} to {DateTo}", dateFrom, dateTo);

        var repo = _unitOfWork.GetRepository<DeliveryTimeSlot, Guid>();
        var spec = new TimeSlotStatsSpec(dateFrom, dateTo);
        var slots = await repo.ListAsync(spec);

        if (!slots.Any())
        {
            return Result<TimeSlotStatsDto>.Success(new TimeSlotStatsDto
            {
                TotalSlots = 0,
                TotalBookings = 0,
                UtilizationRate = 0,
                PeakHour = new TimeOnly(0, 0)
            });
        }

        var totalCapacity = slots.Sum(s => s.MaxCapacity);
        var totalBookings = slots.Sum(s => s.CurrentBookings);

        // Find peak hour by grouping by start time
        var peakGroup = slots
            .GroupBy(s => s.StartTime)
            .OrderByDescending(g => g.Sum(s => s.CurrentBookings))
            .First();

        return Result<TimeSlotStatsDto>.Success(new TimeSlotStatsDto
        {
            TotalSlots = slots.Count,
            TotalBookings = totalBookings,
            UtilizationRate = totalCapacity > 0 ? Math.Round((decimal)totalBookings / totalCapacity * 100, 2) : 0,
            PeakHour = peakGroup.Key
        });
    }

    private static TimeSlotDto MapToDto(DeliveryTimeSlot s) => new()
    {
        Id = s.Id,
        Date = s.Date,
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        MaxCapacity = s.MaxCapacity,
        CurrentBookings = s.CurrentBookings,
        AvailableSlots = s.MaxCapacity - s.CurrentBookings,
        PriceMultiplier = s.PriceMultiplier,
        IsAvailable = s.IsActive && s.CurrentBookings < s.MaxCapacity
    };
}

internal class TimeSlotsByDateSpec : BaseSpecification<DeliveryTimeSlot>
{
    public TimeSlotsByDateSpec(DateOnly date, Guid? regionId = null)
    {
        SetCriteria(s => s.Date == date
            && s.IsActive
            && (!regionId.HasValue || s.RegionId == regionId.Value));
        SetOrderBy(s => s.StartTime);
    }
}

internal class TimeSlotStatsSpec : BaseSpecification<DeliveryTimeSlot>
{
    public TimeSlotStatsSpec(DateOnly? dateFrom, DateOnly? dateTo)
    {
        SetCriteria(s => s.IsActive
            && (!dateFrom.HasValue || s.Date >= dateFrom.Value)
            && (!dateTo.HasValue || s.Date <= dateTo.Value));
    }
}
