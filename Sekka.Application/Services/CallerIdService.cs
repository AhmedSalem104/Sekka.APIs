using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class CallerIdService : ICallerIdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CallerIdService> _logger;

    public CallerIdService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CallerIdService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CallerIdDto>> LookupAsync(Guid driverId, string phone)
    {
        var normalizedPhone = EgyptianPhoneHelper.Normalize(phone);
        var repo = _unitOfWork.GetRepository<CallerIdNote, Guid>();
        var spec = new CallerIdByPhoneSpec(driverId, normalizedPhone);
        var notes = await repo.ListAsync(spec);
        var note = notes.FirstOrDefault();

        // Also check if phone belongs to an existing customer
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customerSpec = new CustomerByPhoneSpec(driverId, normalizedPhone);
        var customers = await customerRepo.ListAsync(customerSpec);
        var customer = customers.FirstOrDefault();

        var dto = new CallerIdDto
        {
            PhoneNumber = normalizedPhone,
            DisplayName = note?.DisplayName ?? customer?.Name,
            ContactType = note?.ContactType ?? Core.Enums.ContactType.Other,
            CustomerName = customer?.Name,
            LastOrderDate = customer?.LastDeliveryDate,
            AverageRating = customer?.AverageRating,
            Note = note?.Note,
            IsBlocked = customer?.IsBlocked ?? false
        };

        return Result<CallerIdDto>.Success(dto);
    }

    public async Task<Result<CallerIdNoteDto>> CreateAsync(Guid driverId, CreateCallerIdNoteDto dto)
    {
        var repo = _unitOfWork.GetRepository<CallerIdNote, Guid>();

        var note = new CallerIdNote
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            PhoneNumber = EgyptianPhoneHelper.Normalize(dto.PhoneNumber),
            ContactType = dto.ContactType,
            DisplayName = dto.DisplayName,
            Note = dto.Note,
            CustomerId = dto.CustomerId,
            PartnerId = dto.PartnerId
        };

        await repo.AddAsync(note);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Caller ID note created for {Phone} by driver {DriverId}", dto.PhoneNumber, driverId);

        return Result<CallerIdNoteDto>.Success(_mapper.Map<CallerIdNoteDto>(note));
    }

    public async Task<Result<CallerIdNoteDto>> UpdateAsync(Guid driverId, Guid id, UpdateCallerIdNoteDto dto)
    {
        var repo = _unitOfWork.GetRepository<CallerIdNote, Guid>();
        var note = await repo.GetByIdAsync(id);

        if (note == null || note.DriverId != driverId)
            return Result<CallerIdNoteDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.ContactType.HasValue) note.ContactType = dto.ContactType.Value;
        if (dto.DisplayName != null) note.DisplayName = dto.DisplayName;
        if (dto.Note != null) note.Note = dto.Note;

        repo.Update(note);
        await _unitOfWork.SaveChangesAsync();

        return Result<CallerIdNoteDto>.Success(_mapper.Map<CallerIdNoteDto>(note));
    }

    public async Task<Result<bool>> DeleteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<CallerIdNote, Guid>();
        var note = await repo.GetByIdAsync(id);

        if (note == null || note.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(note);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Caller ID note {Id} deleted by driver {DriverId}", id, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<TruecallerLookupDto>> TruecallerLookupAsync(string phone)
    {
        var normalizedPhone = EgyptianPhoneHelper.Normalize(phone);

        // Internal lookup: check if this phone exists in our customer database (across all drivers)
        // This provides a basic "caller ID" without relying on external APIs
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var spec = new CustomerByPhoneGlobalSpec(normalizedPhone);
        var customers = await customerRepo.ListAsync(spec);

        var totalOrders = customers.Sum(c => c.TotalDeliveries);
        var avgRating = customers.Count > 0 ? (double)customers.Average(c => c.AverageRating) : 0;
        var isBlocked = customers.Any(c => c.IsBlocked);

        var riskLevel = isBlocked ? "High"
            : totalOrders == 0 ? "Medium"
            : avgRating >= 3.5 ? "Low"
            : "Medium";

        var result = new TruecallerLookupDto
        {
            Phone = normalizedPhone,
            Name = customers.FirstOrDefault(c => c.Name != null)?.Name,
            SpamScore = isBlocked ? 80 : 0,
            IsVerified = totalOrders >= 3,
            InternalCustomer = customers.Count > 0,
            InternalRating = avgRating > 0 ? avgRating : null,
            InternalOrdersCount = totalOrders,
            RiskLevel = riskLevel
        };

        return Result<TruecallerLookupDto>.Success(result);
    }
}

// ── Specifications ──

internal class CustomerByPhoneGlobalSpec : BaseSpecification<Customer>
{
    public CustomerByPhoneGlobalSpec(string phone)
    {
        SetCriteria(c => c.Phone == phone);
    }
}

internal class CallerIdByPhoneSpec : BaseSpecification<CallerIdNote>
{
    public CallerIdByPhoneSpec(Guid driverId, string phone)
    {
        SetCriteria(c => c.DriverId == driverId && c.PhoneNumber == phone);
    }
}
