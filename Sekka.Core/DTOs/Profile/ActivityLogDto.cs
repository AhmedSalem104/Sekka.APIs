namespace Sekka.Core.DTOs.Profile;

public class ActivityLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
}
