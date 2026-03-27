namespace Sekka.Core.DTOs.System;

public class RegionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? NameEn { get; set; }
    public Guid? ParentRegionId { get; set; }
    public double? CenterLatitude { get; set; }
    public double? CenterLongitude { get; set; }
    public double? RadiusKm { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRegionDto
{
    public string Name { get; set; } = null!;
    public string? NameEn { get; set; }
    public Guid? ParentRegionId { get; set; }
    public double? CenterLatitude { get; set; }
    public double? CenterLongitude { get; set; }
    public double? RadiusKm { get; set; }
}

public class UpdateRegionDto
{
    public string? Name { get; set; }
    public string? NameEn { get; set; }
    public Guid? ParentRegionId { get; set; }
    public double? CenterLatitude { get; set; }
    public double? CenterLongitude { get; set; }
    public double? RadiusKm { get; set; }
    public bool? IsActive { get; set; }
}
