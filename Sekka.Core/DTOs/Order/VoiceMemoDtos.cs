namespace Sekka.Core.DTOs.Order;

public class VoiceMemoDto
{
    public Guid Id { get; set; }
    public string AudioUrl { get; set; } = null!;
    public string? Transcription { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
}
