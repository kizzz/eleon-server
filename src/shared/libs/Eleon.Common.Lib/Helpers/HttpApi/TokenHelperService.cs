using EleonsoftAbp.Auth;
using EleonsoftSdk.modules.Helpers.Module;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.HttpApi.Helpers;
public static class TokenHelperService
{
  private static readonly SemaphoreSlim updateTokensSemaphore = new SemaphoreSlim(1, 1);

  public static async Task<string> GetAccessToken(string accessTokenType, EleoncoreSdkConfig sdkConfig, Func<string>? getUserId)
  {
    if (string.IsNullOrEmpty(accessTokenType) || accessTokenType == "oauth")
    {
      if (!sdkConfig.UseOAuthAuthorization)
      {
        throw new Exception("OAuth Authorization is not configured in the SDK.");
      }

      var userId = getUserId?.Invoke();

      if (string.IsNullOrWhiteSpace(userId))
      {
        return string.Empty;
      }

      if (TokenCacheService.TryGetValue(userId, out var _userOAuthToken))
      {
        return _userOAuthToken ?? string.Empty;
      }

      return string.Empty;
    }
    else if (accessTokenType == "api")
    {
      return await GetSdkKey(sdkConfig);
    }
    throw new ArgumentException(nameof(accessTokenType));
  }

  public static void SetOidcToken(string userId, string accessToken)
  {
    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(accessToken))
    {
      return;
    }

    TokenCacheService.AddOrUpdate(userId, accessToken, TimeSpan.FromMinutes(15));
  }

  public static async Task<string> GetSdkKey(EleoncoreSdkConfig sdkConfig)
  {
    if (!sdkConfig.UseApiAuthorization)
    {
      throw new Exception("API Authorization is not configured in the SDK.");
    }

    if (TokenCacheService.TryGetValue(sdkConfig.ApiKey, out var accessToken) && !string.IsNullOrWhiteSpace(accessToken))
    {
      return accessToken;
    }

    await updateTokensSemaphore.WaitAsync();

    try
    {
      if (!TokenCacheService.TryGetValue(sdkConfig.ApiKey, out accessToken) || string.IsNullOrWhiteSpace(accessToken))
      {
        accessToken = await RequestSdkAccessToken(sdkConfig.ClientId, sdkConfig.ClientSecret, sdkConfig.ApiAuthUrl, sdkConfig.ApiKey, sdkConfig.ApiKeySecret, sdkConfig.IgnoreSslValidation);
        TokenCacheService.TryAdd(sdkConfig.ApiKey, accessToken);
      }
    }
    finally
    {
      updateTokensSemaphore.Release();
    }

    return accessToken;
  }

  private static HttpClient CreateHttpClient(bool ignoreSsl)
  {
    if (ignoreSsl)
    {
      var handler = new HttpClientHandler()
      {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
      };
      return new HttpClient(handler);
    }
    else
    {
      return new HttpClient();
    }
  }

  public static async Task<string> RequestSdkAccessToken(string clientId, string clientSecret, string authUrl, string sdkKey, string apiKeySecret, bool ignoreSsl)
  {
    using var client = CreateHttpClient(ignoreSsl);

    var discoveryDocument = await client.GetDiscoveryDocumentAsync(authUrl);

    if (discoveryDocument.IsError)
      throw new Exception(discoveryDocument.Error);

    var (Nonce, Timestamp, Signature) = ApiKeySignatureHelper.GenerateSignature(apiKeySecret);

    var tokenResponse = await client.RequestTokenAsync(new TokenRequest
    {
      Address = discoveryDocument.TokenEndpoint,
      ClientId = clientId,
      ClientSecret = clientSecret,
      GrantType = "x_api_key",
      Parameters = {
                        KeyValuePair.Create("api_key", sdkKey),
                        KeyValuePair.Create("api_nonce", Nonce),
                        KeyValuePair.Create("api_signature", Signature),
                        KeyValuePair.Create("api_timestamp", Timestamp)
                    },
    });

    if (tokenResponse.IsError)
    {
      throw new Exception("Error while authorizing Eleoncore SDK. Error:" + tokenResponse.Error + "; Description: " + tokenResponse.ErrorDescription);
    }

    if (string.IsNullOrEmpty(tokenResponse.AccessToken))
    {
      throw new Exception("Eleoncore authorization did not yield an access token.");
    }

    return tokenResponse.AccessToken;
  }

  private static class TokenCacheService
  {
    private static TimeSpan DefaultExpirationTimeSpan { get; set; } = TimeSpan.FromMinutes(24 * 60);
    private static readonly ConcurrentDictionary<string, TokenInfo> _cacheStore = new ConcurrentDictionary<string, TokenInfo>();

    public static bool TryAdd(string key, string token, TimeSpan? expiresIn = null)
    {
      return _cacheStore.TryAdd(key, new TokenInfo(token, DateTime.UtcNow + (expiresIn ?? DefaultExpirationTimeSpan)));
    }

    public static bool TryGetValue(string key, out string? token)
    {
      if (!_cacheStore.TryGetValue(key, out var tokenInfo))
      {
        token = null;
        return false;
      }

      if (DateTime.UtcNow >= tokenInfo.Expiration)
      {
        Remove(key);
        token = null;
        return false;
      }

      token = tokenInfo.Token;
      return true;
    }

    public static void Remove(string key)
    {
      _cacheStore.Remove(key, out var _);
    }

    public static void AddOrUpdate(string key, string token, TimeSpan? expiresIn = null)
    {
      _cacheStore.AddOrUpdate(key, new TokenInfo(token, DateTime.UtcNow + (expiresIn ?? DefaultExpirationTimeSpan)), (k, v) => new TokenInfo(token, DateTime.UtcNow + (expiresIn ?? DefaultExpirationTimeSpan)));
    }

    private record TokenInfo(string Token, DateTime Expiration);
  }
}
