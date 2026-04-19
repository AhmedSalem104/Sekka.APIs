using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
    private readonly UserManager<Driver> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AccountManagementService> _logger;

    public AccountManagementService(IUnitOfWork unitOfWork, IMapper mapper, ISmsService smsService, UserManager<Driver> userManager, IConfiguration config, ILogger<AccountManagementService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _smsService = smsService;
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }

    public async Task<Result<bool>> RequestAccountDeletionAsync(Guid driverId, DeleteAccountDto dto)
    {
        var repo = _unitOfWork.GetRepository<AccountDeletionRequest, Guid>();

        // Use fake OTP code in dev mode, random 6-digit code in production
        var useFake = _config.GetValue<bool>("OtpSettings:UseFakeOtpInDev");
        var confirmationCode = useFake
            ? (_config["OtpSettings:FakeOtpCode"] ?? "1234")
            : Random.Shared.Next(100000, 999999).ToString();

        var request = new AccountDeletionRequest
        {
            DriverId = driverId,
            Reason = dto.Reason,
            Status = DeletionRequestStatus.Pending,
            ConfirmationCode = confirmationCode
        };

        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

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

        if (request.ConfirmationCode != dto.OtpCode)
            return Result<bool>.BadRequest(ErrorMessages.InvalidConfirmationCode);

        // Mark deletion request as confirmed
        request.Status = DeletionRequestStatus.Confirmed;
        request.ConfirmedAt = DateTime.UtcNow;
        repo.Update(request);

        // Soft-delete: Deactivate the driver account
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver != null)
        {
            // 1. Deactivate account
            driver.IsActive = false;
            driver.IsOnline = false;

            // 2. Anonymize phone to free up the number for re-registration
            var deletedSuffix = $"_DELETED_{DateTime.UtcNow:yyyyMMddHHmmss}";
            driver.UserName = driver.PhoneNumber + deletedSuffix;
            driver.NormalizedUserName = (driver.PhoneNumber + deletedSuffix).ToUpper();
            driver.PhoneNumber = driver.PhoneNumber + deletedSuffix;
            driver.PhoneNumberConfirmed = false;

            // 3. Lock account to prevent login
            driver.LockoutEnabled = true;
            driver.LockoutEnd = DateTimeOffset.MaxValue;

            // 4. Keep all data intact (orders, wallet, settlements, etc.)
            driver.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(driver);

            // 5. Revoke all sessions
            var sessionRepo = _unitOfWork.GetRepository<ActiveSession, Guid>();
            var sessions = await sessionRepo.ListAsync(new ActiveSessionsByDriverSpec(driverId));
            foreach (var session in sessions)
                sessionRepo.Delete(session);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Account soft-deleted for driver {DriverId}. Phone freed for re-registration.", driverId);
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
