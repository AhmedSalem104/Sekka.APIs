using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Account;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class AccountManagementService : IAccountManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISmsService _smsService;
    private readonly ILogger<AccountManagementService> _logger;

    public AccountManagementService(IUnitOfWork unitOfWork, IMapper mapper, ISmsService smsService, ILogger<AccountManagementService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<Result<bool>> RequestAccountDeletionAsync(Guid driverId, DeleteAccountDto dto)
    {
        var repo = _unitOfWork.GetRepository<AccountDeletionRequest, Guid>();
        var confirmationCode = Random.Shared.Next(100000, 999999).ToString();

        var request = new AccountDeletionRequest
        {
            DriverId = driverId,
            Reason = dto.Reason,
            Status = DeletionRequestStatus.Pending,
            ConfirmationCode = confirmationCode
        };

        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        // TODO: Send confirmation code via SMS
        _logger.LogInformation("Account deletion requested for driver {DriverId}, code: {Code}", driverId, confirmationCode);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ConfirmAccountDeletionAsync(Guid driverId, ConfirmDeletionDto dto)
    {
        var repo = _unitOfWork.GetRepository<AccountDeletionRequest, Guid>();
        var requests = await repo.ListAsync(new PendingDeletionRequestSpec(driverId));
        var request = requests.FirstOrDefault();

        if (request == null)
            return Result<bool>.NotFound(ErrorMessages.NoPendingDeletionRequest);

        if (request.ConfirmationCode != dto.ConfirmationCode)
            return Result<bool>.BadRequest(ErrorMessages.InvalidConfirmationCode);

        request.Status = DeletionRequestStatus.Confirmed;
        request.ConfirmedAt = DateTime.UtcNow;
        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Account deletion confirmed for driver {DriverId}", driverId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<ActiveSessionDto>>> GetActiveSessionsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<ActiveSession, Guid>();
        var sessions = await repo.ListAsync(new ActiveSessionsByDriverSpec(driverId));
        return Result<List<ActiveSessionDto>>.Success(_mapper.Map<List<ActiveSessionDto>>(sessions));
    }

    public async Task<Result<bool>> TerminateSessionAsync(Guid driverId, Guid sessionId)
    {
        var repo = _unitOfWork.GetRepository<ActiveSession, Guid>();
        var session = await repo.GetByIdAsync(sessionId);

        if (session == null || session.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.SessionNotFound);

        repo.Delete(session);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> LogoutAllDevicesAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<ActiveSession, Guid>();
        var sessions = await repo.ListAsync(new ActiveSessionsByDriverSpec(driverId));

        foreach (var session in sessions)
            repo.Delete(session);

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("All sessions terminated for driver {DriverId}", driverId);

        return Result<bool>.Success(true);
    }
}

internal class PendingDeletionRequestSpec : Sekka.Core.Specifications.BaseSpecification<AccountDeletionRequest>
{
    public PendingDeletionRequestSpec(Guid driverId)
    {
        SetCriteria(r => r.DriverId == driverId && r.Status == DeletionRequestStatus.Pending);
        SetOrderByDescending(r => r.CreatedAt);
        AsNoTracking = false;
    }
}

internal class ActiveSessionsByDriverSpec : Sekka.Core.Specifications.BaseSpecification<ActiveSession>
{
    public ActiveSessionsByDriverSpec(Guid driverId)
    {
        SetCriteria(s => s.DriverId == driverId);
        SetOrderByDescending(s => s.LastActiveAt);
    }
}
