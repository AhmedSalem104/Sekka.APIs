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

public class DisputeService : IDisputeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DisputeService> _logger;

    public DisputeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DisputeService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DisputeDto>> CreateAsync(Guid driverId, CreateDisputeDto dto)
    {
        // Verify the order exists and belongs to the driver
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var order = await orderRepo.GetByIdAsync(dto.OrderId);

        if (order is null || order.DriverId != driverId)
            return Result<DisputeDto>.NotFound(ErrorMessages.ItemNotFound);

        var repo = _unitOfWork.GetRepository<OrderDispute, Guid>();

        var dispute = new OrderDispute
        {
            Id = Guid.NewGuid(),
            OrderId = dto.OrderId,
            DriverId = driverId,
            DisputeType = dto.DisputeType,
            Status = DisputeStatus.Open,
            Description = dto.Description,
            EvidenceUrls = dto.EvidenceUrls
        };

        await repo.AddAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Dispute {DisputeId} created for order {OrderId} by driver {DriverId}",
            dispute.Id, dto.OrderId, driverId);

        return Result<DisputeDto>.Success(_mapper.Map<DisputeDto>(dispute));
    }

    public async Task<Result<List<DisputeDto>>> GetDisputesAsync(Guid orderId)
    {
        var repo = _unitOfWork.GetRepository<OrderDispute, Guid>();
        var spec = new DisputesByOrderSpec(orderId);
        var disputes = await repo.ListAsync(spec);

        var dtos = _mapper.Map<List<DisputeDto>>(disputes);
        return Result<List<DisputeDto>>.Success(dtos);
    }
}

// ── Specifications ──

internal class DisputesByOrderSpec : BaseSpecification<OrderDispute>
{
    public DisputesByOrderSpec(Guid orderId)
    {
        SetCriteria(d => d.OrderId == orderId);
        SetOrderByDescending(d => d.CreatedAt);
    }
}
