using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Financial;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class RefundService : IRefundService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RefundService> _logger;

    public RefundService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RefundService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RefundDto>> CreateAsync(Guid driverId, CreateRefundDto dto)
    {
        // Verify the order exists and belongs to the driver
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(dto.OrderId);

        if (order is null || order.DriverId != driverId)
            return Result<RefundDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.Amount > order.Amount)
            return Result<RefundDto>.BadRequest("مبلغ الاسترداد أكبر من مبلغ الطلب");

        var repo = _unitOfWork.GetRepository<RefundRequest, Guid>();

        var refund = new RefundRequest
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            DriverId = driverId,
            Amount = dto.Amount,
            RefundReason = dto.RefundReason,
            Status = RefundStatus.Pending,
            Description = dto.Description
        };

        await repo.AddAsync(refund);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Refund {RefundId} created for order {OrderId} by driver {DriverId}, amount: {Amount}",
            refund.Id, dto.OrderId, driverId, dto.Amount);

        return Result<RefundDto>.Success(_mapper.Map<RefundDto>(refund));
    }

    public async Task<Result<List<RefundDto>>> GetRefundsAsync(Guid orderId)
    {
        var repo = _unitOfWork.GetRepository<RefundRequest, Guid>();
        var spec = new RefundsByOrderSpec(orderId);
        var refunds = await repo.ListAsync(spec);

        var dtos = _mapper.Map<List<RefundDto>>(refunds);
        return Result<List<RefundDto>>.Success(dtos);
    }
}

// ── Specifications ──

internal class RefundsByOrderSpec : BaseSpecification<RefundRequest>
{
    public RefundsByOrderSpec(Guid orderId)
    {
        SetCriteria(r => r.OrderId == orderId);
        SetOrderByDescending(r => r.CreatedAt);
    }
}
