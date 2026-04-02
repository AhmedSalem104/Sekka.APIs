using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Interfaces.Services;
using System.Text.RegularExpressions;

namespace Sekka.Application.Services;

public class AzureSpeechService : ISpeechToTextService
{
    private readonly string _subscriptionKey;
    private readonly string _region;
    private readonly ILogger<AzureSpeechService> _logger;
    private readonly HttpClient _httpClient;

    public AzureSpeechService(IConfiguration config, ILogger<AzureSpeechService> logger, HttpClient httpClient)
    {
        _subscriptionKey = config["AzureSpeech:SubscriptionKey"]
            ?? throw new InvalidOperationException("AzureSpeech:SubscriptionKey is not configured");
        _region = config["AzureSpeech:Region"] ?? "westeurope";
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<Result<SpeechTranscriptionResult>> TranscribeAsync(Stream audioStream, string fileName, string? fieldHint = null)
    {
        if (audioStream == null || audioStream.Length == 0)
            return Result<SpeechTranscriptionResult>.BadRequest("No audio data provided");

        try
        {
            // Azure Speech REST API endpoint
            var language = "ar-EG";
            var url = $"https://{_region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language={language}&format=detailed";

            // Read audio into memory
            using var memoryStream = new MemoryStream();
            await audioStream.CopyToAsync(memoryStream);
            var audioBytes = memoryStream.ToArray();

            // Determine content type from file extension
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            var contentType = ext switch
            {
                ".wav" => "audio/wav; codecs=audio/pcm",
                ".mp3" => "audio/mpeg",
                ".m4a" => "audio/mp4",
                ".ogg" => "audio/ogg; codecs=opus",
                ".opus" => "audio/ogg; codecs=opus",
                ".webm" => "audio/webm; codecs=opus",
                _ => "audio/wav"
            };

            _logger.LogInformation("Sending audio to Azure STT: format={Format}, size={Size}bytes, language={Language}",
                contentType, audioBytes.Length, language);

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            request.Headers.Add("Accept-Language", language);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new ByteArrayContent(audioBytes);
            request.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Azure Speech API response: {StatusCode} - {Body}", response.StatusCode, responseBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Azure Speech API error: {StatusCode} - {Body}", response.StatusCode, responseBody);
                return Result<SpeechTranscriptionResult>.BadRequest("خطأ في خدمة التعرف على الصوت");
            }

            // Parse response
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            var recognitionStatus = root.GetProperty("RecognitionStatus").GetString();

            if (recognitionStatus != "Success")
            {
                _logger.LogWarning("Speech not recognized: {Status}", recognitionStatus);
                return recognitionStatus switch
                {
                    "NoMatch" => Result<SpeechTranscriptionResult>.BadRequest("لم يتم التعرف على الكلام - حاول مرة تانية"),
                    "InitialSilenceTimeout" => Result<SpeechTranscriptionResult>.BadRequest("مفيش صوت في التسجيل - حاول مرة تانية"),
                    _ => Result<SpeechTranscriptionResult>.BadRequest($"فشل التعرف على الصوت: {recognitionStatus}")
                };
            }

            // Get best result from NBest array
            var nBest = root.GetProperty("NBest");
            var bestResult = nBest[0];
            var transcription = bestResult.GetProperty("Display").GetString()?.Trim() ?? "";
            var confidence = bestResult.GetProperty("Confidence").GetDouble();

            _logger.LogInformation("Speech transcribed: {Text} (confidence: {Confidence}, field: {Field})",
                transcription, confidence, fieldHint ?? "none");

            var parsed = ParseByFieldHint(transcription, fieldHint);

            return Result<SpeechTranscriptionResult>.Success(new SpeechTranscriptionResult
            {
                Text = transcription,
                ParsedValue = parsed,
                Confidence = confidence,
                Language = "ar-EG"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transcribe audio");
            return Result<SpeechTranscriptionResult>.BadRequest("فشل في تحويل الصوت لنص");
        }
    }

    /// <summary>Smart parsing based on field hint</summary>
    private static string? ParseByFieldHint(string text, string? fieldHint)
    {
        if (string.IsNullOrEmpty(fieldHint)) return null;

        return fieldHint.ToLowerInvariant() switch
        {
            "phone" => ExtractPhoneNumber(text),
            "amount" => ExtractAmount(text),
            "itemcount" => ExtractNumber(text),
            _ => null
        };
    }

    private static string? ExtractPhoneNumber(string text)
    {
        var digitsOnly = Regex.Replace(text, @"[^\d]", "");
        var match = Regex.Match(digitsOnly, @"(01[0-9]{9})");
        if (match.Success) return match.Value;

        var arabicDigitMap = new Dictionary<string, string>
        {
            {"صفر", "0"}, {"واحد", "1"}, {"اتنين", "2"}, {"اثنين", "2"},
            {"تلاته", "3"}, {"ثلاثة", "3"}, {"تلاتة", "3"},
            {"اربعه", "4"}, {"أربعة", "4"}, {"اربعة", "4"},
            {"خمسه", "5"}, {"خمسة", "5"},
            {"سته", "6"}, {"ستة", "6"},
            {"سبعه", "7"}, {"سبعة", "7"},
            {"تمنيه", "8"}, {"ثمانية", "8"}, {"تمانية", "8"}, {"تمانيه", "8"},
            {"تسعه", "9"}, {"تسعة", "9"},
            {"عشره", "10"}, {"عشرة", "10"}
        };

        var result = text;
        foreach (var kvp in arabicDigitMap)
            result = result.Replace(kvp.Key, kvp.Value);

        var converted = Regex.Replace(result, @"[^\d]", "");
        match = Regex.Match(converted, @"(01[0-9]{9})");
        return match.Success ? match.Value : digitsOnly.Length >= 10 ? digitsOnly : null;
    }

    private static string? ExtractAmount(string text)
    {
        var match = Regex.Match(text, @"[\d]+\.?[\d]*");
        if (match.Success) return match.Value;
        return ConvertArabicNumberWords(text);
    }

    private static string? ExtractNumber(string text)
    {
        var match = Regex.Match(text, @"[\d]+");
        if (match.Success) return match.Value;
        return ConvertArabicNumberWords(text);
    }

    private static string? ConvertArabicNumberWords(string text)
    {
        var normalizedText = text.Trim();

        var numberMap = new Dictionary<string, int>
        {
            {"واحد", 1}, {"اتنين", 2}, {"اثنين", 2}, {"تلاته", 3}, {"تلاتة", 3}, {"ثلاثة", 3},
            {"اربعه", 4}, {"أربعة", 4}, {"اربعة", 4}, {"خمسه", 5}, {"خمسة", 5},
            {"سته", 6}, {"ستة", 6}, {"سبعه", 7}, {"سبعة", 7},
            {"تمنيه", 8}, {"ثمانية", 8}, {"تمانية", 8}, {"تمانيه", 8},
            {"تسعه", 9}, {"تسعة", 9}, {"عشره", 10}, {"عشرة", 10},
            {"عشرين", 20}, {"تلاتين", 30}, {"ثلاثين", 30},
            {"اربعين", 40}, {"أربعين", 40}, {"خمسين", 50},
            {"ستين", 60}, {"سبعين", 70}, {"تمانين", 80}, {"ثمانين", 80}, {"تسعين", 90},
            {"مية", 100}, {"ميه", 100}, {"مئة", 100}, {"ميتين", 200}, {"مئتين", 200},
            {"تلتميه", 300}, {"ربعميه", 400}, {"خمسميه", 500},
            {"الف", 1000}, {"ألف", 1000}
        };

        foreach (var kvp in numberMap)
        {
            if (normalizedText.Contains(kvp.Key))
            {
                var total = 0;
                foreach (var part in numberMap)
                {
                    if (normalizedText.Contains(part.Key))
                        total += part.Value;
                }
                return total > 0 ? total.ToString() : null;
            }
        }

        return null;
    }
}
