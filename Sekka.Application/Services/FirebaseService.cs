using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Enums;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Interfaces.Services;
using Sekka.Persistence.Entities;

namespace Sekka.Application.Services;

public class FirebaseService : IFirebaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FirebaseService> _logger;
    private readonly IConfiguration _config;

    public FirebaseService(IUnitOfWork unitOfWork, ILogger<FirebaseService> logger, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _config = config;

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        if (FirebaseApp.DefaultInstance != null) return;

        try
        {
            var credentialsPath = _config["Firebase:CredentialsPath"];
            if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
            {
                _logger.LogWarning("Firebase credentials file not found at: {Path}", credentialsPath);
                return;
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialsPath),
                ProjectId = _config["Firebase:ProjectId"]
            });

            _logger.LogInformation("Firebase initialized successfully for project: {ProjectId}", _config["Firebase:ProjectId"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase");
        }
    }

    public async Task<Result<bool>> SendPushAsync(Guid driverId, string title, string body, Dictionary<string, string>? data = null)
    {
        var tokens = await GetActiveTokensAsync(driverId);
        if (tokens.Count == 0)
        {
            _logger.LogWarning("No active FCM tokens found for driver {DriverId}", driverId);
            return Result<bool>.Success(true); // Not an error — driver just has no devices
        }

        var successCount = 0;
        foreach (var token in tokens)
        {
            var sent = await SendToTokenAsync(token, title, body, data);
            if (sent) successCount++;
        }

        _logger.LogInformation("Push sent to driver {DriverId}: {Success}/{Total} devices", driverId, successCount, tokens.Count);
        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> SendPushToManyAsync(IEnumerable<Guid> driverIds, string title, string body, Dictionary<string, string>? data = null)
    {
        var totalSent = 0;
        foreach (var driverId in driverIds)
        {
            var result = await SendPushAsync(driverId, title, body, data);
            if (result.IsSuccess) totalSent++;
        }

        _logger.LogInformation("Push sent to {Count} drivers", totalSent);
        return Result<int>.Success(totalSent);
    }

    public async Task<Result<int>> SendBroadcastAsync(string title, string body, Dictionary<string, string>? data = null)
    {
        var tokenRepo = _unitOfWork.GetRepository<DeviceToken, Guid>();
        var allTokens = await tokenRepo.ListAsync(new ActiveDeviceTokensSpec());

        if (allTokens.Count == 0)
        {
            _logger.LogWarning("No active FCM tokens found for broadcast");
            return Result<int>.Success(0);
        }

        var tokenStrings = allTokens.Select(t => t.Token).Distinct().ToList();
        var successCount = 0;

        // FCM supports max 500 tokens per multicast
        foreach (var batch in tokenStrings.Chunk(500))
        {
            var message = new MulticastMessage
            {
                Tokens = batch.ToList(),
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default",
                        ChannelId = "sekka_default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        Badge = 1
                    }
                },
                Data = data
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                successCount += response.SuccessCount;

                if (response.FailureCount > 0)
                {
                    await HandleFailedTokensAsync(batch.ToList(), response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send broadcast batch");
            }
        }

        _logger.LogInformation("Broadcast sent: {Success}/{Total} tokens", successCount, tokenStrings.Count);
        return Result<int>.Success(successCount);
    }

    public async Task<Result<bool>> RegisterTokenAsync(Guid driverId, string token, string platform)
    {
        var repo = _unitOfWork.GetRepository<DeviceToken, Guid>();

        // Deactivate old tokens with the same token string (might belong to another driver)
        var existingTokens = await repo.ListAsync(new DeviceTokenByTokenSpec(token));
        foreach (var existing in existingTokens)
        {
            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;
            repo.Update(existing);
        }

        // Create new token
        var deviceToken = new DeviceToken
        {
            Id = Guid.NewGuid(),
            DriverId = driverId,
            Token = token,
            Platform = Enum.TryParse<DevicePlatform>(platform, true, out var p) ? p : DevicePlatform.Android,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(deviceToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("FCM token registered for driver {DriverId}, platform: {Platform}", driverId, platform);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveTokenAsync(Guid driverId, string token)
    {
        var repo = _unitOfWork.GetRepository<DeviceToken, Guid>();
        var tokens = await repo.ListAsync(new DeviceTokenByDriverAndTokenSpec(driverId, token));

        foreach (var t in tokens)
        {
            t.IsActive = false;
            t.UpdatedAt = DateTime.UtcNow;
            repo.Update(t);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("FCM token removed for driver {DriverId}", driverId);
        return Result<bool>.Success(true);
    }

    // ── Private Helpers ──

    private async Task<List<string>> GetActiveTokensAsync(Guid driverId)
    {
        var repo = _unitOfWork.GetRepository<DeviceToken, Guid>();
        var tokens = await repo.ListAsync(new ActiveTokensByDriverSpec(driverId));
        return tokens.Select(t => t.Token).ToList();
    }

    private async Task<bool> SendToTokenAsync(string token, string title, string body, Dictionary<string, string>? data)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            _logger.LogWarning("Firebase not initialized — skipping push");
            return false;
        }

        var message = new Message
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification
                {
                    Sound = "default",
                    ChannelId = "sekka_default"
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Sound = "default",
                    Badge = 1
                }
            },
            Data = data
        };

        try
        {
            var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogDebug("FCM sent successfully: {MessageId}", messageId);
            return true;
        }
        catch (FirebaseMessagingException ex) when (
            ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
            ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
        {
            _logger.LogWarning("Invalid FCM token, deactivating: {Token}", token.Length > 20 ? token[..20] + "..." : token);
            await DeactivateTokenAsync(token);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send FCM to token");
            return false;
        }
    }

    private async Task DeactivateTokenAsync(string token)
    {
        var repo = _unitOfWork.GetRepository<DeviceToken, Guid>();
        var tokens = await repo.ListAsync(new DeviceTokenByTokenSpec(token));
        foreach (var t in tokens)
        {
            t.IsActive = false;
            t.UpdatedAt = DateTime.UtcNow;
            repo.Update(t);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleFailedTokensAsync(List<string> tokens, BatchResponse response)
    {
        for (int i = 0; i < response.Responses.Count; i++)
        {
            if (!response.Responses[i].IsSuccess &&
                response.Responses[i].Exception?.MessagingErrorCode is
                    MessagingErrorCode.Unregistered or MessagingErrorCode.InvalidArgument)
            {
                await DeactivateTokenAsync(tokens[i]);
            }
        }
    }
}

// ── Specifications ──

internal class ActiveTokensByDriverSpec : Sekka.Core.Specifications.BaseSpecification<DeviceToken>
{
    public ActiveTokensByDriverSpec(Guid driverId)
    {
        SetCriteria(t => t.DriverId == driverId && t.IsActive);
    }
}

internal class ActiveDeviceTokensSpec : Sekka.Core.Specifications.BaseSpecification<DeviceToken>
{
    public ActiveDeviceTokensSpec()
    {
        SetCriteria(t => t.IsActive);
    }
}

internal class DeviceTokenByTokenSpec : Sekka.Core.Specifications.BaseSpecification<DeviceToken>
{
    public DeviceTokenByTokenSpec(string token)
    {
        SetCriteria(t => t.Token == token);
    }
}

internal class DeviceTokenByDriverAndTokenSpec : Sekka.Core.Specifications.BaseSpecification<DeviceToken>
{
    public DeviceTokenByDriverAndTokenSpec(Guid driverId, string token)
    {
        SetCriteria(t => t.DriverId == driverId && t.Token == token);
    }
}
