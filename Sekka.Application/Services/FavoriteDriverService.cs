using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Social;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class FavoriteDriverService : IFavoriteDriverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<Driver> _userManager;
    private readonly ILogger<FavoriteDriverService> _logger;
    private const int MaxFavorites = 30;

    public FavoriteDriverService(IUnitOfWork unitOfWork, IMapper mapper,
        UserManager<Driver> userManager, ILogger<FavoriteDriverService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<List<FavoriteDriverDto>>> GetFavoritesAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<FavoriteDriver, Guid>();
        var spec = new FavoriteDriversByOwnerSpec(driverId);
        var favorites = await repo.ListAsync(spec);
        var dtos = _mapper.Map<List<FavoriteDriverDto>>(favorites);
        return Result<List<FavoriteDriverDto>>.Success(dtos);
    }

    public async Task<Result<FavoriteDriverDto>> AddFavoriteAsync(Guid driverId, AddFavoriteDriverDto dto)
    {
        var normalizedPhone = EgyptianPhoneHelper.Normalize(dto.Phone);
        if (!EgyptianPhoneHelper.IsValid(normalizedPhone))
            return Result<FavoriteDriverDto>.BadRequest(ErrorMessages.InvalidEgyptianPhone);

        var repo = _unitOfWork.GetRepository<FavoriteDriver, Guid>();

        // Check duplicate
        var existingSpec = new FavoriteDriverByPhoneSpec(driverId, normalizedPhone);
        var existing = (await repo.ListAsync(existingSpec)).FirstOrDefault();
        if (existing != null)
            return Result<FavoriteDriverDto>.Conflict("الزميل مضاف بالفعل");

        // Check max limit
        var allSpec = new FavoriteDriversByOwnerSpec(driverId);
        var count = (await repo.ListAsync(allSpec)).Count;
        if (count >= MaxFavorites)
            return Result<FavoriteDriverDto>.BadRequest($"الحد الأقصى للمفضلين هو {MaxFavorites}");

        // Lookup if this phone belongs to a registered driver
        var linkedDriver = await _userManager.Users
            .Where(d => d.PhoneNumber == normalizedPhone && d.Id != driverId)
            .Select(d => new { d.Id })
            .FirstOrDefaultAsync();

        var favorite = new FavoriteDriver
        {
            DriverId = driverId,
            Name = dto.Name,
            Phone = normalizedPhone,
            LinkedDriverId = linkedDriver?.Id,
            IsAppUser = linkedDriver != null
        };

        await repo.AddAsync(favorite);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Driver {DriverId} added favorite colleague {Phone}", driverId, normalizedPhone);

        return Result<FavoriteDriverDto>.Success(_mapper.Map<FavoriteDriverDto>(favorite));
    }

    public async Task<Result<bool>> RemoveFavoriteAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<FavoriteDriver, Guid>();
        var favorite = await repo.GetByIdAsync(id);

        if (favorite == null || favorite.DriverId != driverId)
            return Result<bool>.NotFound("الزميل المفضل غير موجود");

        repo.Delete(favorite);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<FavoriteDriverDto>> RefreshAsync(Guid driverId, Guid id)
    {
        var repo = _unitOfWork.GetRepository<FavoriteDriver, Guid>();
        var favorite = await repo.GetByIdAsync(id);

        if (favorite == null || favorite.DriverId != driverId)
            return Result<FavoriteDriverDto>.NotFound("الزميل المفضل غير موجود");

        // Re-check if this phone now belongs to a registered driver
        var linkedDriver = await _userManager.Users
            .Where(d => d.PhoneNumber == favorite.Phone && d.Id != driverId)
            .Select(d => new { d.Id })
            .FirstOrDefaultAsync();

        favorite.LinkedDriverId = linkedDriver?.Id;
        favorite.IsAppUser = linkedDriver != null;
        repo.Update(favorite);
        await _unitOfWork.SaveChangesAsync();

        return Result<FavoriteDriverDto>.Success(_mapper.Map<FavoriteDriverDto>(favorite));
    }
}

// ── Specifications ──

internal class FavoriteDriversByOwnerSpec : BaseSpecification<FavoriteDriver>
{
    public FavoriteDriversByOwnerSpec(Guid driverId)
    {
        SetCriteria(f => f.DriverId == driverId);
        SetOrderByDescending(f => f.CreatedAt);
    }
}

internal class FavoriteDriverByPhoneSpec : BaseSpecification<FavoriteDriver>
{
    public FavoriteDriverByPhoneSpec(Guid driverId, string phone)
    {
        SetCriteria(f => f.DriverId == driverId && f.Phone == phone);
    }
}
