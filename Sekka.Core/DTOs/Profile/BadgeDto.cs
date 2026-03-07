namespace Sekka.Core.DTOs.Profile;

public class BadgeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string IconUrl { get; set; } = null!;
    public DateTime EarnedAt { get; set; }
    public string Category { get; set; } = null!;
}
