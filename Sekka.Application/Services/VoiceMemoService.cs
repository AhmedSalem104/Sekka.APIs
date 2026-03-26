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

public class VoiceMemoService : IVoiceMemoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<VoiceMemoService> _logger;

    public VoiceMemoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<VoiceMemoService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<VoiceMemoDto>> UploadMemoAsync(Guid driverId, Guid? orderId, Guid? customerId, Stream audioStream, string fileName)
    {
        var audioUrl = $"/uploads/voice-memos/{driverId}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var memo = new VoiceMemo
        {
            DriverId = driverId,
            OrderId = orderId,
            CustomerId = customerId,
            AudioUrl = audioUrl
        };

        var repo = _unitOfWork.GetRepository<VoiceMemo, Guid>();
        await repo.AddAsync(memo);
        await _unitOfWork.SaveChangesAsync();

        return Result<VoiceMemoDto>.Success(_mapper.Map<VoiceMemoDto>(memo));
    }

    public async Task<Result<List<VoiceMemoDto>>> GetMemosAsync(Guid driverId, Guid? orderId, Guid? customerId)
    {
        var repo = _unitOfWork.GetRepository<VoiceMemo, Guid>();
        var spec = new VoiceMemosByDriverSpec(driverId, orderId, customerId);
        var memos = await repo.ListAsync(spec);
        return Result<List<VoiceMemoDto>>.Success(_mapper.Map<List<VoiceMemoDto>>(memos));
    }

    public async Task<Result<bool>> DeleteMemoAsync(Guid driverId, Guid memoId)
    {
        var repo = _unitOfWork.GetRepository<VoiceMemo, Guid>();
        var memo = await repo.GetByIdAsync(memoId);

        if (memo == null || memo.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(memo);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

internal class VoiceMemosByDriverSpec : BaseSpecification<VoiceMemo>
{
    public VoiceMemosByDriverSpec(Guid driverId, Guid? orderId, Guid? customerId)
    {
        if (orderId.HasValue)
            SetCriteria(v => v.DriverId == driverId && v.OrderId == orderId.Value);
        else if (customerId.HasValue)
            SetCriteria(v => v.DriverId == driverId && v.CustomerId == customerId.Value);
        else
            SetCriteria(v => v.DriverId == driverId);

        SetOrderByDescending(v => v.CreatedAt);
    }
}
