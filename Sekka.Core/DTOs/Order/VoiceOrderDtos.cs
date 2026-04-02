namespace Sekka.Core.DTOs.Order;

/// <summary>Response from voice transcription</summary>
public class TranscriptionResponseDto
{
    public string Text { get; set; } = null!;
    public string? ParsedValue { get; set; }
    public double Confidence { get; set; }
    public string FieldHint { get; set; } = null!;
}
