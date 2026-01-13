using EleonsoftAbp.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SharedModule.HttpApi.Helpers;
public static class SdkHttpClientFactory
{
  public static string AUTH_SCHEME = "Bearer";
  public static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(100);

  private static ConcurrentDictionary<string, Lazy<SdkHttpClient>> Clients { get; } = new(StringComparer.Ordinal);

  /// <summary>Creates a new HttpClient with base address, auth & headers pre-configured.</summary>
  public static HttpClient Create(ApiConfigurator apiConfigurator, string accessTokenType, string httpClientName)
  {
    var lazy = Clients.GetOrAdd(
        $"{httpClientName}:{accessTokenType}",
        _ => new Lazy<SdkHttpClient>(() =>
        {
          var client = new SdkHttpClient(apiConfigurator, accessTokenType, httpClientName);
          return client;
        }, LazyThreadSafetyMode.ExecutionAndPublication));

    return lazy.Value;
  }

  private sealed class AuthorizationHandler : DelegatingHandler
  {
    private readonly EleoncoreSdkConfig _sdkConfig;
    private readonly string _tokenType;
        private readonly Func<string>? _userIdAccessor;
        private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuthorizationHandler(EleoncoreSdkConfig sdkConfig, string tokenType, IHttpContextAccessor httpContextAccessor, Func<string>? userIdAccessor)
    {
      _sdkConfig = sdkConfig;
      _tokenType = tokenType;
        _userIdAccessor = userIdAccessor;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
      var _accessToken = await TokenHelperService.GetAccessToken(_tokenType, _sdkConfig, () => _userIdAccessor?.Invoke() ?? _httpContextAccessor.HttpContext?.User.GetUserId());
      // Authorization (only set if caller didn’t)
      if (request.Headers.Authorization is null)
      {
        // Assumes your existing static service:
        //   public static class AuthManager { public static Task<string> GetAccessToken(string type); }
        if (!string.IsNullOrWhiteSpace(_accessToken))
          request.Headers.Authorization = new AuthenticationHeaderValue(AUTH_SCHEME, _accessToken);
      }

      SetTenantHeaders(request);

      return await base.SendAsync(request, ct).ConfigureAwait(false);
    }

    private void SetTenantHeaders(HttpRequestMessage request)
    {
      if (_httpContextAccessor?.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Forwarded-Host", out var originalOrigin))
      {
        request.Headers.Add("X-Forwarded-Host", originalOrigin.ToString());
      }
      else if (_httpContextAccessor?.HttpContext != null && _httpContextAccessor.HttpContext.Request.Host.HasValue)
      {
        request.Headers.Add("X-Forwarded-Host", _httpContextAccessor.HttpContext.Request.Host.Value);
      }
    }
  }

  private class SdkHttpClient : HttpClient
  {
    private readonly string _accessTokenType;
    private readonly string _httpClientName;

    public bool IsDisposed { get; private set; } = false;

    public SdkHttpClient(ApiConfigurator apiConfigurator, string accessTokenType, string httpClientName) : base(GetHandler(apiConfigurator, accessTokenType))
    {
      _accessTokenType = accessTokenType;
      _httpClientName = httpClientName;

      this.BaseAddress = BuildBaseAddress(apiConfigurator.SdkConfig);
      this.Timeout = DEFAULT_TIMEOUT;
    }

    protected override void Dispose(bool disposing)
    {
      var key = $"{_httpClientName}:{_accessTokenType}";

      // Remove only if the dictionary still maps to THIS instance
      if (Clients.TryGetValue(key, out var lazy) &&
          lazy.IsValueCreated &&
          ReferenceEquals(lazy.Value, this))
      {
        Clients.TryRemove(key, out _);
      }

      base.Dispose(disposing);
    }

    private static HttpMessageHandler GetHandler(ApiConfigurator apiConfigurator, string accessTokenType)
    {
      var pipeline = new AuthorizationHandler(apiConfigurator.SdkConfig, accessTokenType, apiConfigurator.HttpContextAccessor, apiConfigurator.UserIdAccessor)
      {
        InnerHandler = new SocketsHttpHandler
        {
          AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
          SslOptions = new System.Net.Security.SslClientAuthenticationOptions
          {
            EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
            RemoteCertificateValidationCallback = apiConfigurator.SdkConfig.IgnoreSslValidation
                              ? (sender, certificate, chain, sslPolicyErrors) => true
                              : null
          }
        }
      };

      return pipeline;
    }

    private static Uri BuildBaseAddress(EleoncoreSdkConfig sdkConfig)
    {
      var basePath = sdkConfig.BasePath ?? string.Empty;
      var root = sdkConfig.BaseHost.TrimEnd('/');
      if (string.IsNullOrWhiteSpace(sdkConfig.BasePath))
        return new Uri(root + "/");

      var path = basePath.StartsWith("/") ? basePath : "/" + basePath;
      path = path.EndsWith("/") ? path[..^1] : path;
      return new Uri(root + path + "/");
    }
  }
}
