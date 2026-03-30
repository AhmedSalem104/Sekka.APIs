using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.System;
using Sekka.Persistence;
using Sekka.Persistence.Entities;

namespace Sekka.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/regions")]
[Authorize(Roles = "Admin")]
public class AdminRegionsController : ControllerBase
{
    private readonly SekkaDbContext _db;

    public AdminRegionsController(SekkaDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetRegions()
    {
        var regions = await _db.Regions.AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.NameEn,
                r.ParentRegionId,
                r.CenterLatitude,
                r.CenterLongitude,
                r.RadiusKm,
                r.IsActive,
                r.CreatedAt,
                ChildCount = r.Children.Count
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(regions));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegionDto dto)
    {
        if (dto.ParentRegionId.HasValue)
        {
            var parentExists = await _db.Regions.AnyAsync(r => r.Id == dto.ParentRegionId.Value);
            if (!parentExists)
                return NotFound(ApiResponse<object>.Fail(ErrorMessages.RegionNotFound));
        }

        var region = new Region
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            NameEn = dto.NameEn,
            ParentRegionId = dto.ParentRegionId,
            CenterLatitude = dto.CenterLatitude,
            CenterLongitude = dto.CenterLongitude,
            RadiusKm = dto.RadiusKm,
            IsActive = true
        };

        _db.Regions.Add(region);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            region.Id,
            region.Name,
            region.NameEn,
            region.ParentRegionId,
            region.CenterLatitude,
            region.CenterLongitude,
            region.RadiusKm,
            region.IsActive
        }, SuccessMessages.RegionCreated));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRegionDto dto)
    {
        var region = await _db.Regions.FindAsync(id);
        if (region is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.RegionNotFound));

        if (dto.Name is not null) region.Name = dto.Name;
        if (dto.NameEn is not null) region.NameEn = dto.NameEn;
        if (dto.ParentRegionId.HasValue) region.ParentRegionId = dto.ParentRegionId;
        if (dto.CenterLatitude.HasValue) region.CenterLatitude = dto.CenterLatitude;
        if (dto.CenterLongitude.HasValue) region.CenterLongitude = dto.CenterLongitude;
        if (dto.RadiusKm.HasValue) region.RadiusKm = dto.RadiusKm;
        if (dto.IsActive.HasValue) region.IsActive = dto.IsActive.Value;

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            region.Id,
            region.Name,
            region.NameEn,
            region.ParentRegionId,
            region.CenterLatitude,
            region.CenterLongitude,
            region.RadiusKm,
            region.IsActive
        }, SuccessMessages.RegionUpdated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var region = await _db.Regions
            .Include(r => r.Children)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (region is null)
            return NotFound(ApiResponse<object>.Fail(ErrorMessages.RegionNotFound));

        if (region.Children.Count > 0)
            return BadRequest(ApiResponse<object>.Fail("لا يمكن حذف منطقة تحتوي على مناطق فرعية"));

        _db.Regions.Remove(region);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Success(true, SuccessMessages.RegionDeleted));
    }
}
