using Sekka.Core.Common;

namespace Sekka.Core.Interfaces.Services;

public interface ISpeechToTextService
{
    /// <summary>Transcribe audio stream to text using Speech-to-Text (Arabic Egyptian)</summary>
    Task<Result<SpeechTranscriptionResult>> TranscribeAsync(Stream audioStream, string fileName, string? fieldHint = null);
}

public class SpeechTranscriptionResult
{
    public string Text { get; set; } = null!;
    public string? ParsedValue { get; set; }
    public double Confidence { get; set; }
    public string Language { get; set; } = "ar-EG";
}
