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

public class PaymentRequestService : IPaymentRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentRequestService> _logger;

    public PaymentRequestService(IUnitOfWork unitOfWork, INotificationService notificationService,
        ILogger<PaymentRequestService> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<List<PaymentRequestDto>>> GetRequestsAsync(Guid driverId, PaymentRequestFilterDto filter)
    {
        var repo = _unitOfWork.GetRepository<PaymentRequest, Guid>();
        var spec = new PaymentRequestsByDriverSpec(driverId, filter.Status, filter.Purpose, filter.DateFrom, filter.DateTo);
        var requests = await repo.ListAsync(spec);

        var dtos = requests
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return Result<List<PaymentRequestDto>>.Success(dtos);
    }

    public async Task<Result<PaymentRequestDto>> GetByIdAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<PaymentRequest, Guid>();
        var request = await repo.GetByIdAsync(id);

        if (request == null || request.DriverId != driverId)
            return Result<PaymentRequestDto>.NotFound(ErrorMessages.ItemNotFound);

        return Result<PaymentRequestDto>.Success(MapToDto(request));
    }

    public async Task<Result<PaymentRequestDto>> CreateAsync(Guid driverId, CreatePaymentRequestDto dto)
    {
        var referenceCode = GenerateReferenceCode();

        var request = new PaymentRequest
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            PaymentPurpose = dto.PaymentPurpose,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            ReferenceCode = referenceCode,
            SenderPhone = dto.SenderPhone,
            SenderName = dto.SenderName,
            Notes = dto.Notes,
            Status = PaymentRequestStatus.Pending,
            RelatedEntityId = dto.RelatedEntityId,
            RelatedEntityType = dto.RelatedEntityType,
            ExpiresAt = DateTime.UtcNow.AddHours(48),
            CreatedAt = DateTime.UtcNow
        };

        var repo = _unitOfWork.GetRepository<PaymentRequest, Guid>();
        await repo.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Payment request {RequestId} created for driver {DriverId}, ref: {RefCode}, amount: {Amount}",
            request.Id, driverId, referenceCode, dto.Amount);

        await _notificationService.CreateAndPushAsync(driverId, NotificationType.CashAlert,
            "طلب دفع جديد", $"تم إنشاء طلب دفع بمبلغ {request.Amount:N2} جنيه — الكود: {request.ReferenceCode}",
            "PAYMENT_REQUEST", request.Id.ToString());

        return Result<PaymentRequestDto>.Success(MapToDto(request));
    }

    public async Task<Result<bool>> UploadProofAsync(Guid driverId, Guid id, Stream stream, string fileName)
    {
        var repo = _unitOfWork.GetRepository<PaymentRequest, Guid>();
        var request = await repo.GetByIdAsync(id);

        if (request == null || request.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        // File upload would be handled by a storage service
        _logger.LogInformation("Proof upload for payment request {Id} by driver {DriverId}, file: {FileName}",
            id, driverId, fileName);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> CancelAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<PaymentRequest, Guid>();
        var request = await repo.GetByIdAsync(id);

        if (request == null || request.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        if (request.Status != PaymentRequestStatus.Pending)
            return Result<bool>.BadRequest(ErrorMessages.InvalidOrderStatus);

        request.Status = PaymentRequestStatus.Cancelled;
        repo.Update(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Payment request {RequestId} cancelled by driver {DriverId}", id, driverId);
        return Result<bool>.Success(true);
    }

    private static string GenerateReferenceCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyMMddHHmm");
        var random = Random.Shared.Next(1000, 9999);
        return $"SKA-{timestamp}-{random}";
    }

    private static PaymentRequestDto MapToDto(PaymentRequest r) => new()
    {
        Id = r.Id,
        DriverId = r.DriverId,
        PaymentPurpose = r.PaymentPurpose,
        Amount = r.Amount,
        PaymentMethod = r.PaymentMethod,
        ReferenceCode = r.ReferenceCode,
        ProofImageUrl = r.ProofImageUrl,
        SenderPhone = r.SenderPhone,
        SenderName = r.SenderName,
        Notes = r.Notes,
        Status = r.Status,
        AdminNotes = r.AdminNotes,
        ReviewedAt = r.ReviewedAt,
        CreatedAt = r.CreatedAt
    };
}

internal class PaymentRequestsByDriverSpec : BaseSpecification<PaymentRequest>
{
    public PaymentRequestsByDriverSpec(Guid driverId, PaymentRequestStatus? status = null, PaymentPurpose? purpose = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        SetCriteria(r => r.DriverId == driverId
            && (!status.HasValue || r.Status == status.Value)
            && (!purpose.HasValue || r.PaymentPurpose == purpose.Value)
            && (!dateFrom.HasValue || r.CreatedAt >= dateFrom.Value)
            && (!dateTo.HasValue || r.CreatedAt <= dateTo.Value));
        SetOrderByDescending(r => r.CreatedAt);
    }
}
