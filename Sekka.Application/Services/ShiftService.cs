using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class ShiftService : IShiftService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShiftService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ShiftDto>> StartShiftAsync(Guid driverId, StartShiftDto dto)
    {
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(driverId);

        if (driver is null)
            return Result<ShiftDto>.NotFound(ErrorMessages.ItemNotFound);

        if (driver.ShiftStartTime.HasValue)
            return Result<ShiftDto>.BadRequest("الوردية قيد التشغيل بالفعل");

        driver.ShiftStartTime = DateTime.UtcNow;
        driver.IsOnline = true;
        driver.LastKnownLatitude = dto.Latitude;
        driver.LastKnownLongitude = dto.Longitude;
        driver.LastLocationUpdate = DateTime.UtcNow;

        driverRepo.Update(driver);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Driver {DriverId} started shift", driverId);

        var shiftDto = new ShiftDto
        {
            Id = driverId,
            DriverId = driverId,
            Status = ShiftStatus.OnShift,
            StartTime = driver.ShiftStartTime.Value,
            StartLatitude = dto.Latitude,
            StartLongitude = dto.Longitude,
            OrdersCompleted = 0,
            EarningsTotal = 0,
            DistanceKm = 0
        };

        return Result<ShiftDto>.Success(shiftDto);
    }

    public async Task<Result<ShiftDto>> EndShiftAsync(Guid driverId)
    {
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(driverId);

        if (driver is null)
            return Result<ShiftDto>.NotFound(ErrorMessages.ItemNotFound);

        if (!driver.ShiftStartTime.HasValue)
            return Result<ShiftDto>.BadRequest("لا توجد وردية نشطة حالياً");

        var shiftStart = driver.ShiftStartTime.Value;
        var shiftEnd = DateTime.UtcNow;

        // Query orders completed during this shift
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new OrdersByDriverInRangeSpec(driverId, shiftStart, shiftEnd);
        var deliveredOrders = await orderRepo.ListAsync(spec);

        var ordersCompleted = deliveredOrders.Count;
        var earningsTotal = deliveredOrders.Sum(o => o.Amount);
        var distanceKm = deliveredOrders.Sum(o => o.DistanceKm ?? 0);

        // Reset driver shift state
        driver.ShiftStartTime = null;
        driver.IsOnline = false;

        driverRepo.Update(driver);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Driver {DriverId} ended shift. Duration: {Duration}min, Orders: {Orders}",
            driverId, (shiftEnd - shiftStart).TotalMinutes, ordersCompleted);

        var shiftDto = new ShiftDto
        {
            Id = driverId,
            DriverId = driverId,
            Status = ShiftStatus.OffShift,
            StartTime = shiftStart,
            EndTime = shiftEnd,
            StartLatitude = driver.LastKnownLatitude ?? 0,
            StartLongitude = driver.LastKnownLongitude ?? 0,
            EndLatitude = driver.LastKnownLatitude,
            EndLongitude = driver.LastKnownLongitude,
            OrdersCompleted = ordersCompleted,
            EarningsTotal = earningsTotal,
            DistanceKm = distanceKm
        };

        return Result<ShiftDto>.Success(shiftDto);
    }

    public async Task<Result<ShiftDto>> GetCurrentShiftAsync(Guid driverId)
    {
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(driverId);

        if (driver is null)
            return Result<ShiftDto>.NotFound(ErrorMessages.ItemNotFound);

        if (!driver.ShiftStartTime.HasValue)
            return Result<ShiftDto>.NotFound("لا توجد وردية نشطة حالياً");

        var shiftStart = driver.ShiftStartTime.Value;
        var now = DateTime.UtcNow;

        // Query orders completed during current shift
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new OrdersByDriverInRangeSpec(driverId, shiftStart, now);
        var deliveredOrders = await orderRepo.ListAsync(spec);

        var shiftDto = new ShiftDto
        {
            Id = driverId,
            DriverId = driverId,
            Status = ShiftStatus.OnShift,
            StartTime = shiftStart,
            StartLatitude = driver.LastKnownLatitude ?? 0,
            StartLongitude = driver.LastKnownLongitude ?? 0,
            OrdersCompleted = deliveredOrders.Count,
            EarningsTotal = deliveredOrders.Sum(o => o.Amount),
            DistanceKm = deliveredOrders.Sum(o => o.DistanceKm ?? 0)
        };

        return Result<ShiftDto>.Success(shiftDto);
    }

    public async Task<Result<ShiftSummaryDto>> GetSummaryAsync(Guid driverId, DateOnly? from, DateOnly? to)
    {
        var driverRepo = _unitOfWork.GetRepository<Driver, Guid>();
        var driver = await driverRepo.GetByIdAsync(driverId);

        if (driver is null)
            return Result<ShiftSummaryDto>.NotFound(ErrorMessages.ItemNotFound);

        var dateFrom = from ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var dateTo = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var startUtc = dateFrom.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endUtc = dateTo.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var spec = new OrdersByDriverInRangeSpec(driverId, startUtc, endUtc);
        var orders = await orderRepo.ListAsync(spec);

        var totalDays = (dateTo.ToDateTime(TimeOnly.MinValue) - dateFrom.ToDateTime(TimeOnly.MinValue)).TotalDays + 1;

        var summary = new ShiftSummaryDto
        {
            TotalShifts = (int)totalDays,
            TotalHoursWorked = totalDays * 8, // Estimate based on period
            TotalOrdersCompleted = orders.Count,
            TotalEarnings = orders.Sum(o => o.Amount),
            TotalDistanceKm = orders.Sum(o => o.DistanceKm ?? 0),
            AverageShiftDurationHours = totalDays > 0 ? (totalDays * 8) / totalDays : 0
        };

        return Result<ShiftSummaryDto>.Success(summary);
    }
}

// ── Specifications ──

internal class OrdersByDriverInRangeSpec : BaseSpecification<Order>
{
    public OrdersByDriverInRangeSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId
            && o.Status == OrderStatus.Delivered
            && o.DeliveredAt >= from
            && o.DeliveredAt <= to);
    }
}
