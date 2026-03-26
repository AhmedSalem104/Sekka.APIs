using Sekka.Core.Common;
using Sekka.Core.DTOs.Financial;

namespace Sekka.Core.Interfaces.Services;

public interface IPaymentRequestService
{
    Task<Result<List<PaymentRequestDto>>> GetRequestsAsync(Guid driverId, PaymentRequestFilterDto filter);
    Task<Result<PaymentRequestDto>> GetByIdAsync(Guid driverId, Guid id);
    Task<Result<PaymentRequestDto>> CreateAsync(Guid driverId, CreatePaymentRequestDto dto);
    Task<Result<bool>> UploadProofAsync(Guid driverId, Guid id, Stream stream, string fileName);
    Task<Result<bool>> CancelAsync(Guid driverId, Guid id);
}
