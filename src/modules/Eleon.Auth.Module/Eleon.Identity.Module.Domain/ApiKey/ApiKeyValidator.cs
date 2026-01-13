using Common.EventBus.Module;
using Commons.Module.Messages.Identity;
using EleonsoftAbp.Messages.ApiKey;
using EleonsoftSdk.modules.Helpers.Module;
using Logging.Module;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;

namespace ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;
public class ApiKeyValidator : ITransientDependency
{
  private readonly static TimeSpan MaxTimestampAge = TimeSpan.FromMinutes(5);
  private readonly static TimeSpan NonceExpiration = TimeSpan.FromMinutes(10);
  private readonly static TimeSpan AllowedMistake = TimeSpan.FromSeconds(10); // if the timestamp is slightly in the future

  private readonly int MinNonceLength = 10;
  private const string NonceCacheKey = "NonceCacheKey";

  private readonly IVportalLogger<ApiKeyValidator> _logger;
  private readonly IDistributedEventBus _apiKeyDomainService;
  private readonly IMemoryCache _memoryCache;
  private readonly List<string> _notValidatedSecretKeys;

  public ApiKeyValidator(
      IVportalLogger<ApiKeyValidator> logger,
      IDistributedEventBus apiKeyDomainService,
      IMemoryCache memoryCache,
      IConfiguration configuration
      )
  {
    _logger = logger;
    _apiKeyDomainService = apiKeyDomainService;
    _memoryCache = memoryCache;
    _notValidatedSecretKeys = configuration.GetSection("ApiKey:NotValidatedSecretKeys").Get<List<string>>() ?? new List<string>();
  }

  public async Task<ApiKeyValidationResult> ValidateApiRequest(string apiKey, string nonce, DateTime timestamp, string signature)
  {
    try
    {
      var key = (await _apiKeyDomainService.RequestAsync<ValidApiKeyReponseMsg>(new GetValidApiKeyRequestMsg
      {
        ApiKey = apiKey,
      })).ApiKey;

      if (key == null)
      {
        return new ApiKeyValidationResult(false, "Api key not found or expired", null);
      }

      if (_notValidatedSecretKeys.Contains(apiKey))
      {
        return new ApiKeyValidationResult(true, null, key);
      }

      {
        // Validate timestamp
        var currentTime = DateTime.UtcNow;
        if (timestamp > (currentTime + AllowedMistake) || currentTime - timestamp > MaxTimestampAge)
        {
          _logger.Log.LogWarning($"Timestamp validation failed. CurrentTime: {currentTime}, Timestamp: {timestamp}, MaxAllowedAge: {MaxTimestampAge}");
          return new ApiKeyValidationResult(false, $"Timestamp is invalid or too old. Max allowed age: {MaxTimestampAge.TotalMinutes} minutes.", null);
        }
      }

      {
        // Nonce replay protection using memory cache
        var cacheKey = $"{NonceCacheKey}:{apiKey}:{nonce}";
        if (string.IsNullOrEmpty(nonce) || nonce.Length < MinNonceLength || _memoryCache.TryGetValue(cacheKey, out _))
        {
          return new ApiKeyValidationResult(false, "Nonce has already been used.", null);
        }

        // Store the nonce with an expiration
        _memoryCache.Set(cacheKey, true, NonceExpiration);
      }

      {
        // Validate signature
        var expectedSignature = ApiKeySignatureHelper.GenerateSignature(key.KeySecret, nonce, timestamp);
        if (expectedSignature.ToUpper() != signature.ToUpper())
        {
          _logger.Log.LogError("Signature validation failed. Expected: {ExpectedSignature}, Provided: {ProvidedSignature}", expectedSignature, signature);
          return new ApiKeyValidationResult(false, "Invalid signature", null);
        }
      }

      return new ApiKeyValidationResult(true, null, key);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
      return new ApiKeyValidationResult(false, "An error occurred while validating the API key.", null);
    }
  }
}
