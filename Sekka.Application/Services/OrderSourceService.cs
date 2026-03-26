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

public class OrderSourceService : IOrderSourceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderSourceService> _logger;

    public OrderSourceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderSourceService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrderSourceTagDto>> GetSourceTagAsync(Guid orderId)
    {
        var repo = _unitOfWork.GetRepository<OrderSourceTag, Guid>();
        var spec = new SourceTagByOrderSpec(orderId);
        var tag = (await repo.ListAsync(spec)).FirstOrDefault();

        if (tag == null)
            return Result<OrderSourceTagDto>.NotFound(ErrorMessages.OrderNotFound);

        return Result<OrderSourceTagDto>.Success(_mapper.Map<OrderSourceTagDto>(tag));
    }

    public async Task<Result<OrderSourceTagDto>> SetSourceTagAsync(Guid orderId, OrderSourceTagDto dto)
    {
        var repo = _unitOfWork.GetRepository<OrderSourceTag, Guid>();
        var spec = new SourceTagByOrderSpec(orderId);
        var existing = (await repo.ListAsync(spec)).FirstOrDefault();

        if (existing != null)
        {
            existing.SourceType = dto.SourceType;
            existing.SourceName = dto.SourceName;
            existing.SourceReference = dto.SourceReference;
            repo.Update(existing);
        }
        else
        {
            var tag = new OrderSourceTag
            {
                OrderId = orderId,
                SourceType = dto.SourceType,
                SourceName = dto.SourceName,
                SourceReference = dto.SourceReference
            };
            await repo.AddAsync(tag);
        }

        await _unitOfWork.SaveChangesAsync();
        var result = (await repo.ListAsync(spec)).First();
        return Result<OrderSourceTagDto>.Success(_mapper.Map<OrderSourceTagDto>(result));
    }

    public Task<Result<List<OrderSourceStatsDto>>> GetMonthlyStatsAsync(Guid driverId, int year, int month)
    {
        return Task.FromResult(Result<List<OrderSourceStatsDto>>.BadRequest(ErrorMessages.FeatureUnderDevelopment("تقارير مصادر الطلبات")));
    }
}

internal class SourceTagByOrderSpec : BaseSpecification<OrderSourceTag>
{
    public SourceTagByOrderSpec(Guid orderId)
    {
        SetCriteria(s => s.OrderId == orderId);
    }
}
