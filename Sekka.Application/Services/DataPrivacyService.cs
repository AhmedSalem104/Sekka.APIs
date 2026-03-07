using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Privacy;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class DataPrivacyService : IDataPrivacyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DataPrivacyService> _logger;

    public DataPrivacyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DataPrivacyService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ConsentDto>>> GetConsentsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<UserConsent, Guid>();
        var consents = await repo.ListAsync(new ConsentsByDriverSpec(driverId));
        return Result<List<ConsentDto>>.Success(_mapper.Map<List<ConsentDto>>(consents));
    }

    public async Task<Result<ConsentDto>> UpdateConsentAsync(Guid driverId, string consentType, UpdateConsentDto dto)
    {
        var repo = _unitOfWork.GetRepository<UserConsent, Guid>();
        var consents = await repo.ListAsync(new ConsentsByDriverSpec(driverId));
        var consent = consents.FirstOrDefault(c => c.ConsentType == consentType);

        if (consent == null)
        {
            consent = new UserConsent
            {
                DriverId = driverId,
                ConsentType = consentType,
                IsGranted = dto.IsGranted,
                GrantedAt = dto.IsGranted ? DateTime.UtcNow : null
            };
            await repo.AddAsync(consent);
        }
        else
        {
            consent.IsGranted = dto.IsGranted;
            if (dto.IsGranted)
            {
                consent.GrantedAt = DateTime.UtcNow;
                consent.RevokedAt = null;
            }
            else
            {
                consent.RevokedAt = DateTime.UtcNow;
            }
            repo.Update(consent);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<ConsentDto>.Success(_mapper.Map<ConsentDto>(consent));
    }

    public async Task<Result<DataExportDto>> RequestDataExportAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<DataDeletionRequest, Guid>();
        var request = new DataDeletionRequest
        {
            DriverId = driverId,
            RequestType = "export",
            Status = DeletionRequestStatus.Pending
        };

        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Data export requested for driver {DriverId}", driverId);

        return Result<DataExportDto>.Success(new DataExportDto
        {
            RequestId = request.Id,
            Status = "Pending",
            RequestedAt = request.CreatedAt
        });
    }

    public async Task<Result<DeletionRequestDto>> RequestDataDeletionAsync(Guid driverId, DeletionRequestDto dto)
    {
        var repo = _unitOfWork.GetRepository<DataDeletionRequest, Guid>();
        var request = new DataDeletionRequest
        {
            DriverId = driverId,
            RequestType = dto.RequestType,
            Status = DeletionRequestStatus.Pending
        };

        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Data deletion requested for driver {DriverId}, type: {Type}", driverId, dto.RequestType);

        return Result<DeletionRequestDto>.Success(new DeletionRequestDto
        {
            RequestType = request.RequestType,
            Status = request.Status
        });
    }

    public async Task<Result<DeletionRequestDto>> GetDeletionStatusAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<DataDeletionRequest, Guid>();
        var requests = await repo.ListAsync(new LatestDeletionRequestSpec(driverId));
        var request = requests.FirstOrDefault();

        if (request == null)
            return Result<DeletionRequestDto>.NotFound(ErrorMessages.NoDeletionRequest);

        return Result<DeletionRequestDto>.Success(new DeletionRequestDto
        {
            RequestType = request.RequestType,
            Status = request.Status
        });
    }
}

internal class ConsentsByDriverSpec : Sekka.Core.Specifications.BaseSpecification<UserConsent>
{
    public ConsentsByDriverSpec(Guid driverId)
    {
        SetCriteria(c => c.DriverId == driverId);
        AsNoTracking = false;
    }
}

internal class LatestDeletionRequestSpec : Sekka.Core.Specifications.BaseSpecification<DataDeletionRequest>
{
    public LatestDeletionRequestSpec(Guid driverId)
    {
        SetCriteria(r => r.DriverId == driverId);
        SetOrderByDescending(r => r.CreatedAt);
    }
}
