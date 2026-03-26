using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class WaitingTimerService : IWaitingTimerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WaitingTimerService> _logger;

    public WaitingTimerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WaitingTimerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<WaitingTimerDto>> StartTimerAsync(Guid driverId, Guid orderId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<WaitingTimerDto>.NotFound(ErrorMessages.OrderNotFound);

        var timerRepo = _unitOfWork.GetRepository<WaitingTimer, Guid>();
        var activeTimers = await timerRepo.ListAsync(new ActiveWaitingTimerSpec(orderId));

        if (activeTimers.Any())
            return Result<WaitingTimerDto>.BadRequest(ErrorMessages.WaitingTimerNotActive);

        var timer = new WaitingTimer
        {
            OrderId = orderId,
            StartTime = DateTime.UtcNow
        };

        await timerRepo.AddAsync(timer);
        await _unitOfWork.SaveChangesAsync();

        return Result<WaitingTimerDto>.Success(_mapper.Map<WaitingTimerDto>(timer));
    }

    public async Task<Result<WaitingTimerDto>> StopTimerAsync(Guid driverId, Guid orderId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<WaitingTimerDto>.NotFound(ErrorMessages.OrderNotFound);

        var timerRepo = _unitOfWork.GetRepository<WaitingTimer, Guid>();
        var activeTimers = await timerRepo.ListAsync(new ActiveWaitingTimerSpec(orderId));

        var activeTimer = activeTimers.FirstOrDefault();
        if (activeTimer == null)
            return Result<WaitingTimerDto>.BadRequest(ErrorMessages.WaitingTimerNotActive);

        activeTimer.EndTime = DateTime.UtcNow;
        activeTimer.DurationSeconds = (int)(activeTimer.EndTime.Value - activeTimer.StartTime).TotalSeconds;

        timerRepo.Update(activeTimer);
        await _unitOfWork.SaveChangesAsync();

        return Result<WaitingTimerDto>.Success(_mapper.Map<WaitingTimerDto>(activeTimer));
    }
}

internal class ActiveWaitingTimerSpec : BaseSpecification<WaitingTimer>
{
    public ActiveWaitingTimerSpec(Guid orderId)
    {
        SetCriteria(t => t.OrderId == orderId && t.EndTime == null);
    }
}
