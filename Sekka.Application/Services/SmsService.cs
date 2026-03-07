using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.Interfaces.Services;

namespace Sekka.Application.Services;

public class SmsService : ISmsService
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _httpClient;

    public SmsService(IDistributedCache cache, IConfiguration config, ILogger<SmsService> logger, HttpClient httpClient)
    {
        _cache = cache;
        _config = config;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<Result<bool>> SendOtpAsync(string phoneNumber)
    {
        var normalized = EgyptianPhoneHelper.Normalize(phoneNumber);
        if (!EgyptianPhoneHelper.IsMobile(normalized))
            return Result<bool>.BadRequest(ErrorMessages.InvalidPhoneNumber);

        var otpSettings = _config.GetSection("OtpSettings");
        var maxAttempts = otpSettings.GetValue<int>("MaxResendPerNumber");

        var cooldownKey = $"Sekka_OTP_RESEND:{normalized}";
        var resendCount = await _cache.GetStringAsync(cooldownKey);
        if (resendCount != null && int.Parse(resendCount) >= maxAttempts)
            return Result<bool>.BadRequest(ErrorMessages.OtpResendLimitReached);

        var useFake = otpSettings.GetValue<bool>("UseFakeOtpInDev");
        var otpCode = useFake
            ? otpSettings["FakeOtpCode"]!
            : Random.Shared.Next(1000, 9999).ToString();

        var otpKey = $"Sekka_OTP:{normalized}";
        var expiryMinutes = otpSettings.GetValue<int>("ExpiryMinutes");
        await _cache.SetStringAsync(otpKey, otpCode, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryMinutes)
        });

        var currentCount = resendCount != null ? int.Parse(resendCount) + 1 : 1;
        await _cache.SetStringAsync(cooldownKey, currentCount.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
        });

        if (!useFake)
        {
            var sent = await SendSmsViaProviderAsync(normalized, ErrorMessages.SmsOtpTemplate(otpCode));
            if (!sent)
                return Result<bool>.BadRequest(ErrorMessages.SmsSendFailed);

            _logger.LogInformation("SMS OTP sent to {Phone}", normalized);
        }
        else
        {
            _logger.LogInformation("Dev mode — OTP for {Phone}: {Otp}", normalized, otpCode);
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> VerifyOtpAsync(string phoneNumber, string otpCode)
    {
        var normalized = EgyptianPhoneHelper.Normalize(phoneNumber);

        var attemptsKey = $"Sekka_OTP_ATTEMPTS:{normalized}";
        var attempts = await _cache.GetStringAsync(attemptsKey);
        var maxAttempts = _config.GetValue<int>("OtpSettings:MaxAttemptsPerNumber");
        if (attempts != null && int.Parse(attempts) >= maxAttempts)
            return Result<bool>.BadRequest(ErrorMessages.OtpAttemptsExceeded);

        var otpKey = $"Sekka_OTP:{normalized}";
        var storedOtp = await _cache.GetStringAsync(otpKey);
        if (storedOtp == null)
            return Result<bool>.BadRequest(ErrorMessages.OtpExpiredOrNotFound);

        if (storedOtp != otpCode)
        {
            var currentAttempts = attempts != null ? int.Parse(attempts) + 1 : 1;
            await _cache.SetStringAsync(attemptsKey, currentAttempts.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });
            return Result<bool>.BadRequest(ErrorMessages.OtpInvalid);
        }

        await _cache.RemoveAsync(otpKey);
        await _cache.RemoveAsync(attemptsKey);

        return Result<bool>.Success(true);
    }

    private async Task<bool> SendSmsViaProviderAsync(string phoneNumber, string message)
    {
        var smsConfig = _config.GetSection("SmsProvider");
        var mobile = phoneNumber.Replace("+", "");

        var url = $"{smsConfig["BaseUrl"]}?username={Uri.EscapeDataString(smsConfig["Username"]!)}" +
                  $"&password={Uri.EscapeDataString(smsConfig["Password"]!)}" +
                  $"&sendername={Uri.EscapeDataString(smsConfig["SenderId"]!)}" +
                  $"&mobiles={mobile}" +
                  $"&message={Uri.EscapeDataString(message)}";

        try
        {
            var response = await _httpClient.PostAsync(url, null);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("SMS Provider response: {Response}", body);

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var first = root.ValueKind == JsonValueKind.Array ? root[0] : root;
            var type = first.GetProperty("type").GetString();
            return type == "success";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {Phone}", phoneNumber);
            return false;
        }
    }
}
