using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Customer;
using Sekka.Core.DTOs.Order;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PagedResult<CustomerListDto>>> GetCustomersAsync(Guid driverId, CustomerFilterDto filter)
    {
        var repo = _unitOfWork.GetRepository<Customer, Guid>();
        var spec = new CustomersByDriverSpec(driverId, filter);
        var items = await repo.ListAsync(spec);
        var countSpec = new CustomersByDriverCountSpec(driverId, filter);
        var total = await repo.CountAsync(countSpec);

        var dtos = _mapper.Map<List<CustomerListDto>>(items);
        return Result<PagedResult<CustomerListDto>>.Success(
            new PagedResult<CustomerListDto>(dtos, total, filter.Page, filter.PageSize));
    }

    public async Task<Result<CustomerDetailDto>> GetByIdAsync(Guid driverId, Guid customerId)
    {
        var repo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await repo.GetByIdAsync(customerId);

        if (customer == null || customer.DriverId != driverId)
            return Result<CustomerDetailDto>.NotFound(ErrorMessages.ItemNotFound);

        return Result<CustomerDetailDto>.Success(_mapper.Map<CustomerDetailDto>(customer));
    }

    public async Task<Result<CustomerDetailDto>> GetByPhoneAsync(Guid driverId, string phone)
    {
        var normalizedPhone = EgyptianPhoneHelper.Normalize(phone);
        var repo = _unitOfWork.GetRepository<Customer, Guid>();
        var spec = new CustomerByPhoneSpec(driverId, normalizedPhone);
        var customers = await repo.ListAsync(spec);
        var customer = customers.FirstOrDefault();

        if (customer == null)
            return Result<CustomerDetailDto>.NotFound(ErrorMessages.ItemNotFound);

        return Result<CustomerDetailDto>.Success(_mapper.Map<CustomerDetailDto>(customer));
    }

    public async Task<Result<CustomerDto>> UpdateAsync(Guid driverId, Guid customerId, UpdateCustomerDto dto)
    {
        var repo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await repo.GetByIdAsync(customerId);

        if (customer == null || customer.DriverId != driverId)
            return Result<CustomerDto>.NotFound(ErrorMessages.ItemNotFound);

        if (dto.Name != null) customer.Name = dto.Name;
        if (dto.Notes != null) customer.Notes = dto.Notes;
        if (dto.PreferredPaymentMethod.HasValue) customer.PreferredPaymentMethod = dto.PreferredPaymentMethod;

        repo.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }

    public async Task<Result<RatingDto>> RateAsync(Guid driverId, Guid customerId, CreateRatingDto dto)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.GetByIdAsync(customerId);

        if (customer == null || customer.DriverId != driverId)
            return Result<RatingDto>.NotFound(ErrorMessages.ItemNotFound);

        var ratingRepo = _unitOfWork.GetRepository<Rating, Guid>();

        var rating = new Rating
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            CustomerId = customerId,
            OrderId = dto.OrderId,
            RatingValue = dto.RatingValue,
            QuickResponse = dto.QuickResponse,
            ClearAddress = dto.ClearAddress,
            RespectfulBehavior = dto.RespectfulBehavior,
            EasyPayment = dto.EasyPayment,
            WrongAddress = dto.WrongAddress,
            NoAnswer = dto.NoAnswer,
            DelayedPickup = dto.DelayedPickup,
            PaymentIssue = dto.PaymentIssue,
            FeedbackText = dto.FeedbackText
        };

        await ratingRepo.AddAsync(rating);

        // Recalculate average rating
        var ratingsSpec = new RatingsByCustomerSpec(driverId, customerId);
        var allRatings = await ratingRepo.ListAsync(ratingsSpec);
        var totalRatings = allRatings.Count + 1; // include the new one
        var sumRatings = allRatings.Sum(r => r.RatingValue) + dto.RatingValue;
        customer.AverageRating = Math.Round((decimal)sumRatings / totalRatings, 2);

        customerRepo.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer {CustomerId} rated by driver {DriverId}: {Rating}", customerId, driverId, dto.RatingValue);

        return Result<RatingDto>.Success(_mapper.Map<RatingDto>(rating));
    }

    public async Task<Result<bool>> BlockAsync(Guid driverId, Guid customerId, BlockCustomerDto dto)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.GetByIdAsync(customerId);

        if (customer == null || customer.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        customer.IsBlocked = true;
        customer.BlockReason = dto.Reason;
        customerRepo.Update(customer);

        var blockedRepo = _unitOfWork.GetRepository<BlockedCustomer, Guid>();
        var blocked = new BlockedCustomer
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            CustomerPhone = customer.Phone,
            CustomerId = customerId,
            Reason = dto.Reason,
            IsCommunityReport = dto.ReportToCommunity,
            BlockedAt = DateTime.UtcNow
        };

        await blockedRepo.AddAsync(blocked);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer {CustomerId} blocked by driver {DriverId}", customerId, driverId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UnblockAsync(Guid driverId, Guid customerId)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.GetByIdAsync(customerId);

        if (customer == null || customer.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        customer.IsBlocked = false;
        customer.BlockReason = null;
        customerRepo.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer {CustomerId} unblocked by driver {DriverId}", customerId, driverId);

        return Result<bool>.Success(true);
    }

    public Task<Result<PagedResult<OrderListDto>>> GetOrdersAsync(Guid driverId, Guid customerId, PaginationDto pagination)
    {
        // TODO: Query orders by customerId using specifications
        return Task.FromResult(Result<PagedResult<OrderListDto>>.Success(
            new PagedResult<OrderListDto>(new List<OrderListDto>(), 0, pagination.Page, pagination.PageSize)));
    }

    public async Task<Result<List<CustomerInterestDto>>> GetInterestsAsync(Guid driverId, Guid customerId)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.GetByIdAsync(customerId);

        if (customer is null || customer.DriverId != driverId)
            return Result<List<CustomerInterestDto>>.NotFound(ErrorMessages.ItemNotFound);

        // Return empty list — interests are populated by the intelligence background service
        var interests = new List<CustomerInterestDto>();
        return Result<List<CustomerInterestDto>>.Success(interests);
    }

    public async Task<Result<CustomerEngagementDto>> GetEngagementAsync(Guid driverId, Guid customerId)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.GetByIdAsync(customerId);

        if (customer is null || customer.DriverId != driverId)
            return Result<CustomerEngagementDto>.NotFound(ErrorMessages.ItemNotFound);

        int? daysSinceLastOrder = customer.LastDeliveryDate.HasValue
            ? (int)(DateTime.UtcNow - customer.LastDeliveryDate.Value).TotalDays
            : null;

        var level = customer.TotalDeliveries switch
        {
            >= 50 => "VIP",
            >= 20 => "متكرر",
            >= 5 => "منتظم",
            _ => "جديد"
        };

        var engagementScore = customer.TotalDeliveries > 0
            ? Math.Min(100m, customer.TotalDeliveries * 5m + customer.AverageRating * 10m)
            : 0m;

        var engagement = new CustomerEngagementDto
        {
            TotalOrders = customer.TotalDeliveries,
            EngagementScore = engagementScore,
            Level = level,
            LastInteraction = customer.LastDeliveryDate,
            DaysSinceLastOrder = daysSinceLastOrder
        };

        return Result<CustomerEngagementDto>.Success(engagement);
    }
}

// ── Specifications ──

internal class CustomersByDriverSpec : BaseSpecification<Customer>
{
    public CustomersByDriverSpec(Guid driverId, CustomerFilterDto filter)
    {
        if (filter.IsBlocked.HasValue && filter.MinRating.HasValue)
            SetCriteria(c => c.DriverId == driverId && c.IsBlocked == filter.IsBlocked.Value && c.AverageRating >= filter.MinRating.Value);
        else if (filter.IsBlocked.HasValue)
            SetCriteria(c => c.DriverId == driverId && c.IsBlocked == filter.IsBlocked.Value);
        else if (filter.MinRating.HasValue)
            SetCriteria(c => c.DriverId == driverId && c.AverageRating >= filter.MinRating.Value);
        else
            SetCriteria(c => c.DriverId == driverId);

        SetOrderByDescending(c => c.LastDeliveryDate!);
        ApplyPaging((filter.Page - 1) * filter.PageSize, filter.PageSize);
    }
}

internal class CustomersByDriverCountSpec : BaseSpecification<Customer>
{
    public CustomersByDriverCountSpec(Guid driverId, CustomerFilterDto filter)
    {
        if (filter.IsBlocked.HasValue && filter.MinRating.HasValue)
            SetCriteria(c => c.DriverId == driverId && c.IsBlocked == filter.IsBlocked.Value && c.AverageRating >= filter.MinRating.Value);
        else if (filter.IsBlocked.HasValue)
            SetCriteria(c => c.DriverId == driverId && c.IsBlocked == filter.IsBlocked.Value);
        else if (filter.MinRating.HasValue)
            SetCriteria(c => c.DriverId == driverId && c.AverageRating >= filter.MinRating.Value);
        else
            SetCriteria(c => c.DriverId == driverId);
    }
}

internal class CustomerByPhoneSpec : BaseSpecification<Customer>
{
    public CustomerByPhoneSpec(Guid driverId, string phone)
    {
        SetCriteria(c => c.DriverId == driverId && c.Phone == phone);
    }
}

internal class RatingsByCustomerSpec : BaseSpecification<Rating>
{
    public RatingsByCustomerSpec(Guid driverId, Guid customerId)
    {
        SetCriteria(r => r.DriverId == driverId && r.CustomerId == customerId);
    }
}
