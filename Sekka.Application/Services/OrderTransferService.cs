using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class OrderTransferService : IOrderTransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderTransferService> _logger;

    public OrderTransferService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderTransferService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrderTransferResponseDto>> TransferAsync(Guid driverId, Guid orderId, TransferOrderDto dto)
    {
        var repo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await repo.GetByIdAsync(orderId);

        if (order == null || order.DriverId != driverId)
            return Result<OrderTransferResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        var transfer = new OrderTransferLog
        {
            OrderId = orderId,
            FromDriverId = driverId,
            ToDriverId = dto.ToDriverId,
            TransferReason = dto.Reason,
            DeepLinkToken = dto.ToDriverId == null ? Guid.NewGuid().ToString("N")[..16] : null,
            Status = dto.ToDriverId.HasValue ? TransferStatus.Pending : TransferStatus.Pending,
            TransferredAt = DateTime.UtcNow
        };

        var transferRepo = _unitOfWork.GetRepository<OrderTransferLog, Guid>();
        await transferRepo.AddAsync(transfer);
        await _unitOfWork.SaveChangesAsync();

        return Result<OrderTransferResponseDto>.Success(_mapper.Map<OrderTransferResponseDto>(transfer));
    }
}
