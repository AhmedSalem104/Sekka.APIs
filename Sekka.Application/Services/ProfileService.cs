using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.DTOs.Profile;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;
using SM = Sekka.Core.Common.Messages.SuccessMessages;

namespace Sekka.Application.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<Driver> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(UserManager<Driver> userManager, IUnitOfWork unitOfWork, ILogger<ProfileService> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DriverProfileDto>> GetProfileAsync(Guid driverId)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null)
            return Result<DriverProfileDto>.NotFound(ErrorMessages.DriverNotFound);

        // Calculate live stats from orders
        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        var allOrdersSpec = new ProfileDriverOrdersSpec(driverId);
        var orders = await orderRepo.ListAsync(allOrdersSpec);

        var totalOrders = orders.Count;
        var delivered = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        var totalDelivered = delivered.Count;
        var totalEarnings = delivered.Sum(o => o.Amount);

        // Today stats
        var todayStart = DateTime.UtcNow.Date;
        var todayOrders = orders.Where(o => o.CreatedAt >= todayStart).ToList();
        var todayDelivered = todayOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();

        // Wallet balance from transactions
        var txRepo = _unitOfWork.GetRepository<WalletTransaction, Guid>();
        var txSpec = new ProfileWalletTransactionsSpec(driverId);
        var transactions = await txRepo.ListAsync(txSpec);
        var lastTx = transactions.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
        var walletBalance = lastTx?.BalanceAfter ?? 0;

        var dto = MapToDto(driver);
        dto.TotalOrders = totalOrders;
        dto.TotalDelivered = totalDelivered;
        dto.WalletBalance = walletBalance;
        dto.TodayOrdersCount = todayOrders.Count;
        dto.TodayEarnings = todayDelivered.Sum(o => o.Amount);
        dto.ShiftStatus = driver.ShiftStartTime.HasValue ? ShiftStatus.OnShift : ShiftStatus.OffShift;
        dto.ShiftStartTime = driver.ShiftStartTime;

        return Result<DriverProfileDto>.Success(dto);
    }

    public async Task<Result<DriverProfileDto>> UpdateProfileAsync(Guid driverId, UpdateProfileDto dto)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null)
            return Result<DriverProfileDto>.NotFound(ErrorMessages.DriverNotFound);

        if (dto.Name != null) driver.Name = dto.Name;
        if (dto.Email != null) driver.Email = dto.Email;
        if (dto.VehicleType.HasValue) driver.VehicleType = dto.VehicleType.Value;
        if (dto.DefaultRegionId.HasValue) driver.DefaultRegionId = dto.DefaultRegionId;
        if (dto.CashAlertThreshold.HasValue) driver.CashAlertThreshold = dto.CashAlertThreshold.Value;
        if (dto.SpeedCompleteMode.HasValue) driver.SpeedCompleteMode = dto.SpeedCompleteMode.Value;
        driver.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(driver);
        return Result<DriverProfileDto>.Success(MapToDto(driver));
    }

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxImageSize = 5 * 1024 * 1024; // 5MB

    public async Task<Result<string>> UploadProfileImageAsync(Guid driverId, Stream imageStream, string fileName)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null) return Result<string>.NotFound(ErrorMessages.DriverNotFound);

        var ext = Path.GetExtension(fileName).ToLower();
        if (!AllowedImageExtensions.Contains(ext))
            return Result<string>.BadRequest("نوع الملف غير مسموح. الأنواع المسموحة: jpg, png, webp");

        var uploadsBase = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "uploads");
        var dir = Path.Combine(uploadsBase, "profiles", driverId.ToString());
        Directory.CreateDirectory(dir);

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var savedName = $"{Guid.NewGuid():N}_{timestamp}{ext}";
        var filePath = Path.Combine(dir, savedName);
        using (var fs = new FileStream(filePath, FileMode.Create))
            await imageStream.CopyToAsync(fs);

        var imageUrl = $"/uploads/profiles/{driverId}/{savedName}";
        driver.ProfileImageUrl = imageUrl;
        driver.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(driver);

        return Result<string>.Success(imageUrl);
    }

    public async Task<Result<bool>> DeleteProfileImageAsync(Guid driverId)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null) return Result<bool>.NotFound(ErrorMessages.DriverNotFound);

        if (!string.IsNullOrEmpty(driver.ProfileImageUrl))
        {
            var uploadsBase = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "uploads");
            var filePath = Path.Combine(uploadsBase, driver.ProfileImageUrl.TrimStart('/').Replace("uploads/", ""));
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        driver.ProfileImageUrl = null;
        driver.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(driver);
        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> UploadLicenseImageAsync(Guid driverId, Stream imageStream, string fileName)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null) return Result<string>.NotFound(ErrorMessages.DriverNotFound);

        var ext = Path.GetExtension(fileName).ToLower();
        if (!AllowedImageExtensions.Contains(ext))
            return Result<string>.BadRequest("نوع الملف غير مسموح. الأنواع المسموحة: jpg, png, webp");

        var uploadsBase = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "uploads");
        var dir = Path.Combine(uploadsBase, "licenses", driverId.ToString());
        Directory.CreateDirectory(dir);

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var savedName = $"{Guid.NewGuid():N}_{timestamp}{ext}";
        var filePath = Path.Combine(dir, savedName);
        using (var fs = new FileStream(filePath, FileMode.Create))
            await imageStream.CopyToAsync(fs);

        var imageUrl = $"/uploads/licenses/{driverId}/{savedName}";
        driver.LicenseImageUrl = imageUrl;
        driver.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(driver);

        return Result<string>.Success(imageUrl);
    }

    public async Task<Result<ProfileCompletionDto>> GetCompletionAsync(Guid driverId)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null)
            return Result<ProfileCompletionDto>.NotFound(ErrorMessages.DriverNotFound);

        var completed = new List<string>();
        var pending = new List<ProfileStepDto>();
        var totalWeight = 0;
        var completedWeight = 0;

        // Name (required, 20%)
        if (!string.IsNullOrEmpty(driver.Name) && driver.Name != ErrorMessages.DefaultDriverName)
        { completed.Add(SM.StepName); completedWeight += 20; }
        else
            pending.Add(new ProfileStepDto { StepName = SM.StepName, StepKey = "name", IsRequired = true, Weight = 20 });
        totalWeight += 20;

        // Vehicle Type (required, 20%)
        if (driver.VehicleType.HasValue)
        { completed.Add(SM.StepVehicleType); completedWeight += 20; }
        else
            pending.Add(new ProfileStepDto { StepName = SM.StepVehicleType, StepKey = "vehicle_type", IsRequired = true, Weight = 20 });
        totalWeight += 20;

        // License Image (required, 20%)
        if (!string.IsNullOrEmpty(driver.LicenseImageUrl))
        { completed.Add(SM.StepLicenseImage); completedWeight += 20; }
        else
            pending.Add(new ProfileStepDto { StepName = SM.StepLicenseImage, StepKey = "license_image", IsRequired = true, Weight = 20 });
        totalWeight += 20;

        // Profile Image (optional, 15%)
        if (!string.IsNullOrEmpty(driver.ProfileImageUrl))
        { completed.Add(SM.StepProfilePhoto); completedWeight += 15; }
        else
            pending.Add(new ProfileStepDto { StepName = SM.StepProfilePhoto, StepKey = "profile_photo", IsRequired = false, Weight = 15 });
        totalWeight += 15;

        // Email (optional, 10%)
        if (!string.IsNullOrEmpty(driver.Email))
        { completed.Add(SM.StepEmail); completedWeight += 10; }
        else
            pending.Add(new ProfileStepDto { StepName = SM.StepEmail, StepKey = "email", IsRequired = false, Weight = 10 });
        totalWeight += 10;

        // Default Region (optional, 15%)
        if (driver.DefaultRegionId.HasValue)
        { completed.Add(SM.StepDefaultRegion); completedWeight += 15; }
        else
            pending.Add(new ProfileStepDto { StepName = SM.StepDefaultRegion, StepKey = "default_region", IsRequired = false, Weight = 15 });
        totalWeight += 15;

        var percentage = totalWeight > 0 ? (completedWeight * 100) / totalWeight : 0;

        return Result<ProfileCompletionDto>.Success(new ProfileCompletionDto
        {
            CompletionPercentage = percentage,
            CompletedSteps = completed,
            PendingSteps = pending,
            IsProfileComplete = percentage >= 100
        });
    }

    public async Task<Result<DriverStatsDto>> GetStatsAsync(Guid driverId, DateTime? fromDate, DateTime? toDate)
    {
        var driver = await _userManager.FindByIdAsync(driverId.ToString());
        if (driver == null)
            return Result<DriverStatsDto>.NotFound(ErrorMessages.DriverNotFound);

        var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
        BaseSpecification<Order> spec = fromDate.HasValue || toDate.HasValue
            ? new ProfileDriverOrdersInRangeSpec(driverId, fromDate ?? DateTime.MinValue, toDate ?? DateTime.UtcNow)
            : new ProfileDriverOrdersSpec(driverId);
        var orders = await orderRepo.ListAsync(spec);

        var totalOrders = orders.Count;
        var delivered = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        var failed = orders.Count(o => o.Status == OrderStatus.Failed);
        var cancelled = orders.Count(o => o.Status == OrderStatus.Cancelled);
        var totalEarnings = delivered.Sum(o => o.Amount);
        var totalCommissions = delivered.Sum(o => o.CommissionAmount);

        // Average delivery time (from PickedUpAt to DeliveredAt)
        var deliveryTimes = delivered
            .Where(o => o.PickedUpAt.HasValue && o.DeliveredAt.HasValue)
            .Select(o => (o.DeliveredAt!.Value - o.PickedUpAt!.Value).TotalMinutes)
            .ToList();
        var avgDeliveryTime = deliveryTimes.Count > 0 ? (decimal)deliveryTimes.Average() : 0;

        // Best day
        var bestDayGroup = delivered
            .Where(o => o.DeliveredAt.HasValue)
            .GroupBy(o => o.DeliveredAt!.Value.Date)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return Result<DriverStatsDto>.Success(new DriverStatsDto
        {
            TotalOrders = totalOrders,
            TotalDelivered = delivered.Count,
            TotalFailed = failed,
            TotalCancelled = cancelled,
            SuccessRate = totalOrders > 0 ? Math.Round((decimal)delivered.Count / totalOrders * 100, 1) : 0,
            TotalEarnings = totalEarnings,
            TotalCommissions = totalCommissions,
            AverageDeliveryTimeMinutes = Math.Round(avgDeliveryTime, 1),
            BestDay = bestDayGroup?.Key,
            BestDayOrders = bestDayGroup?.Count() ?? 0
        });
    }

    public Task<Result<List<BadgeDto>>> GetBadgesAsync(Guid driverId)
    {
        // TODO: Query from DriverAchievements table
        return Task.FromResult(Result<List<BadgeDto>>.Success(new List<BadgeDto>()));
    }

    public Task<Result<PagedResult<ActivityLogDto>>> GetActivityLogAsync(Guid driverId, PaginationDto pagination)
    {
        // TODO: Query activity logs
        return Task.FromResult(Result<PagedResult<ActivityLogDto>>.Success(
            new PagedResult<ActivityLogDto>(new List<ActivityLogDto>(), 0, pagination.Page, pagination.PageSize)));
    }

    public async Task<Result<List<EmergencyContactDto>>> GetEmergencyContactsAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<EmergencyContact, Guid>();
        var spec = new EmergencyContactsByDriverSpec(driverId);
        var contacts = await repo.ListAsync(spec);

        var dtos = contacts.Select(c => new EmergencyContactDto
        {
            Id = c.Id,
            Name = c.Name,
            Phone = c.Phone,
            Relationship = c.Relationship
        }).ToList();

        return Result<List<EmergencyContactDto>>.Success(dtos);
    }

    public async Task<Result<EmergencyContactDto>> AddEmergencyContactAsync(Guid driverId, CreateEmergencyContactDto dto)
    {
        var repo = _unitOfWork.GetRepository<EmergencyContact, Guid>();

        var contact = new EmergencyContact
        {
            DriverId = driverId,
            Name = dto.Name,
            Phone = EgyptianPhoneHelper.Normalize(dto.Phone),
            Relationship = dto.Relationship
        };

        await repo.AddAsync(contact);
        await _unitOfWork.SaveChangesAsync();

        return Result<EmergencyContactDto>.Success(new EmergencyContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            Phone = contact.Phone,
            Relationship = contact.Relationship
        });
    }

    public async Task<Result<bool>> DeleteEmergencyContactAsync(Guid driverId, Guid contactId)
    {
        var repo = _unitOfWork.GetRepository<EmergencyContact, Guid>();
        var contact = await repo.GetByIdAsync(contactId);

        if (contact == null || contact.DriverId != driverId)
            return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        repo.Delete(contact);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public Task<Result<SubscriptionDto>> GetSubscriptionAsync(Guid driverId)
    {
        // TODO: Query from Subscriptions table
        return Task.FromResult(Result<SubscriptionDto>.NotFound(ErrorMessages.NoCurrentSubscription));
    }

    public Task<Result<SubscriptionDto>> UpgradeSubscriptionAsync(Guid driverId, UpgradeSubscriptionDto dto)
    {
        // TODO: Process subscription upgrade
        return Task.FromResult(Result<SubscriptionDto>.BadRequest(ErrorMessages.SubscriptionsUnderDev));
    }

    public Task<Result<List<DriverAchievementDto>>> GetAchievementsAsync(Guid driverId)
    {
        return Task.FromResult(Result<List<DriverAchievementDto>>.Success(new List<DriverAchievementDto>()));
    }

    public Task<Result<List<ChallengeDto>>> GetChallengesAsync(Guid driverId)
    {
        return Task.FromResult(Result<List<ChallengeDto>>.Success(new List<ChallengeDto>()));
    }

    public Task<Result<LeaderboardDto>> GetLeaderboardAsync(Guid driverId)
    {
        return Task.FromResult(Result<LeaderboardDto>.Success(new LeaderboardDto()));
    }

    public Task<Result<PagedResult<ExpenseDto>>> GetExpensesAsync(Guid driverId, ExpenseFilterDto filter)
    {
        return Task.FromResult(Result<PagedResult<ExpenseDto>>.Success(
            new PagedResult<ExpenseDto>(new List<ExpenseDto>(), 0, filter.Page, filter.PageSize)));
    }

    public Task<Result<ExpenseDto>> AddExpenseAsync(Guid driverId, CreateExpenseDto dto)
    {
        // TODO: Add to Expenses table
        return Task.FromResult(Result<ExpenseDto>.Success(new ExpenseDto
        {
            Id = Guid.NewGuid(),
            Category = dto.Category,
            Amount = dto.Amount,
            Description = dto.Description,
            Date = dto.Date ?? DateTime.UtcNow
        }));
    }

    private static DriverProfileDto MapToDto(Driver driver) => new()
    {
        Id = driver.Id,
        Name = driver.Name,
        Phone = driver.PhoneNumber ?? "",
        Email = driver.Email,
        ProfileImageUrl = driver.ProfileImageUrl,
        LicenseImageUrl = driver.LicenseImageUrl,
        VehicleType = driver.VehicleType ?? Sekka.Core.Enums.VehicleType.Motorcycle,
        IsOnline = driver.IsOnline,
        CashOnHand = driver.CashOnHand,
        TotalPoints = driver.TotalPoints,
        Level = driver.Level,
        JoinedAt = driver.CreatedAt,
        ReferralCode = driver.Id.ToString()[..8].ToUpper()
    };
}

internal class EmergencyContactsByDriverSpec : BaseSpecification<EmergencyContact>
{
    public EmergencyContactsByDriverSpec(Guid driverId)
    {
        SetCriteria(c => c.DriverId == driverId);
        SetOrderBy(c => c.SortOrder);
    }
}

internal class ProfileDriverOrdersSpec : BaseSpecification<Order>
{
    public ProfileDriverOrdersSpec(Guid driverId)
    {
        SetCriteria(o => o.DriverId == driverId);
    }
}

internal class ProfileDriverOrdersInRangeSpec : BaseSpecification<Order>
{
    public ProfileDriverOrdersInRangeSpec(Guid driverId, DateTime from, DateTime to)
    {
        SetCriteria(o => o.DriverId == driverId && o.CreatedAt >= from && o.CreatedAt <= to);
    }
}

internal class ProfileWalletTransactionsSpec : BaseSpecification<WalletTransaction>
{
    public ProfileWalletTransactionsSpec(Guid driverId)
    {
        SetCriteria(t => t.DriverId == driverId);
    }
}
