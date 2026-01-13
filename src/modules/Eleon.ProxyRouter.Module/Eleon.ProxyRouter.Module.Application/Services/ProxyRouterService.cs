using Eleon.ProxyRouter.Module.Eleon.ProxyRouter.Module.Domain.Shared.Helpers;
using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using ProxyRouter.Minimal.HttpApi.ErrorHandling.Exceptions;
using ProxyRouter.Minimal.HttpApi.Models.Constants;
using ProxyRouter.Minimal.HttpApi.Models.Messages;
using ProxyRouter.Minimal.HttpApi.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Policy;
using Volo.Abp.Threading;
using VPortal.Masav.Feature.Module.Transformers;
using VPortal.ProxyRouter;
using Yarp.ReverseProxy.Forwarder;


namespace ProxyRouter.Module.Services
{
  /// <summary>
  /// Extension interface for diagnostics and lifecycle hooks.
  /// </summary>
  public interface IProxyExtension
  {
    Task OnBeforeForwardAsync(HttpContext context, Location location);
    Task OnAfterForwardAsync(HttpContext context, Location location, ForwarderError error, TimeSpan duration);
  }

  /// <summary>
  /// Handles routing and proxy forwardings for all incoming requests.
  /// Supports Angular, Virtual, and API-type locations.
  /// </summary>
  public class ProxyRouterService
  {
    private readonly IHttpForwarder _httpForwarder;
    private readonly ILogger<ProxyRouterService> _logger;
    private readonly ConfigurationCacheService _configurationCacheService;
    private readonly ILocationProvider _moduleSettingsService;
    private readonly IEnumerable<IProxyExtension> _extensions;

    public ProxyRouterService(
        IHttpForwarder httpForwarder,
        ILogger<ProxyRouterService> logger,
        ConfigurationCacheService configurationCacheService,
        ILocationProvider moduleSettingsService,
        IEnumerable<IProxyExtension>? extensions = null)
    {
      _httpForwarder = httpForwarder;
      _logger = logger;
      _configurationCacheService = configurationCacheService;
      _moduleSettingsService = moduleSettingsService;
      _extensions = extensions ?? Enumerable.Empty<IProxyExtension>();
    }

    public async Task HandleRequest(HttpContext context, Location location)
    {
      try
      {
        // Check authentication first
        if (location.IsAuthorized && !context.User.Identity?.IsAuthenticated == true)
        {
          _logger.LogWarning("Unauthorized access attempt to protected location. Path={Path}", location.Path);

          throw new UnauthorizedAccessException("Authentication required")
              .WithStatusCode(StatusCodes.Status401Unauthorized)
              .WithFriendlyMessage("You must be logged in to access this resource")
              .WithData("Location", location.Path);
        }

        // Check authorization policy if specified
        if (!string.IsNullOrEmpty(location.RequiredPolicy))
        {
          var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();
          var authResult = await authorizationService.AuthorizeAsync(
              context.User,
              null,
              location.RequiredPolicy);

          if (!authResult.Succeeded)
          {
            _logger.LogWarning("Authorization failed. User={User}, Policy={Policy}, Path={Path}",
                context.User.Identity?.Name ?? "Anonymous",
                location.RequiredPolicy,
                location.Path);

            throw new UnauthorizedAccessException("Access denied")
                .WithStatusCode(StatusCodes.Status403Forbidden)
                .WithFriendlyMessage("You don't have permission to access this resource")
                .WithData("RequiredPolicy", location.RequiredPolicy)
                .WithData("Location", location.Path);
          }

          _logger.LogDebug("Authorization successful. Policy={Policy}, User={User}",
              location.RequiredPolicy, context.User.Identity?.Name);
        }

        var route = context.Request.GetEncodedPathAndQuery();

        switch (location.Type)
        {
          case LocationType.Angular:
            await HandleAngularRequest(context, location, route);
            break;

          case LocationType.Virtual:
            HandleVirtualRedirect(context, location);
            break;

          default:
            await HandleProxyForward(context, location);
            break;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "ProxyRouterService errored: {message}", ex.Message);

        if (ex is ProxyException proxyEx)
          throw proxyEx.WithData("Location", location);

        throw new ProxyException(ex.Message, ex)
            .WithData("Location", location)
            .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyInternalError);
      }
    }

    private async Task HandleAngularRequest(HttpContext context, Location location, string route)
    {
      ModuleSettingsDto settings = null;
      try
      {
        settings = await _moduleSettingsService.GetModuleProperties($"{context.Request.Scheme}://{context.Request.Host}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to retrieve module settings for Angular request at route {Route}", route);
      }

      var application = settings?.ClientApplications?.FirstOrDefault(a => a.Id.ToString() == location.ResourceId);

      var isPwa = Convert.ToBoolean(application?.Properties?.FirstOrDefault(p => p.Key == "IsPwa")?.Value ?? "false");
      var pwaManifest = application?.Properties?.FirstOrDefault(p => p.Key == "PwaManifest")?.Value ?? "";

      if (location.RemotePath == "/manifest.webmanifest" && isPwa)
      {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(pwaManifest);
        return;
      }

      var isSw = Convert.ToBoolean(application?.Properties?.FirstOrDefault(p => p.Key == "UseServiceWorker")?.Value ?? "false");
      var swConfig = application?.Properties?.FirstOrDefault(p => p.Key == "ServiceWorkerConfig")?.Value ?? "";

      if (location.RemotePath == "/ngsw.json" && isSw)
      {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(swConfig);
        return;
      }

      var htmlContent = HtmlHelper.GenerateHtml(application?.Name ?? "Eleoncore", application?.Path?.EnsureEndsWith("/") ?? location.CheckedPath ?? "/", isPwa ? "<link rel=\"manifest\" href=\"manifest.webmanifest\">" : "");
      _logger.LogDebug("Generated Angular HTML for route {Route}", location.RemotePath);
      context.Response.ContentType = "text/html";
      await context.Response.WriteAsync(htmlContent);
    }

    private static void HandleVirtualRedirect(HttpContext context, Location location)
    {
      if (string.IsNullOrEmpty(location.DefaultRedirect))
        throw new ProxyException("Redirect URL was empty");

      context.Response.Redirect(location.DefaultRedirect);
    }

    private async Task HandleProxyForward(HttpContext context, Location location)
    {
      var url = location.SourceUrl + (string.IsNullOrEmpty(location.RemotePath)
                ? string.Empty
                : location.RemotePath.EnsureStartsWith("/"));

      try
      {
        // Check for circular requests using configured domain
        var appDomain = _configurationCacheService.GetSetting("App:Domain");
        if (!string.IsNullOrEmpty(appDomain))
        {
          var uri = new Uri(url);
          var targetHost = uri.Host;

          if (targetHost.Equals(appDomain, StringComparison.OrdinalIgnoreCase) ||
              $"{targetHost}:{uri.Port}".Equals(appDomain, StringComparison.OrdinalIgnoreCase))
          {
            _logger.LogWarning("Circular request detected. Target URL matches application domain: {Url}, Domain: {AppDomain}",
                url, appDomain);

            throw new ProxyException("Circular request found")
                .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyForbiddenRequest)
                .WithFriendlyMessage("Access to local network protected resources is forbidden")
                .WithData("TargetUrl", url)
                .WithData("AppDomain", appDomain);
          }
        }

        //if (new Uri(url).Host.Equals(context.Request.Host.Host, StringComparison.OrdinalIgnoreCase)) // todo host + port
        //{
        //  _logger.Log.LogWarning("Circular request detected. Target URL matches request host: {Url}, Host: {RequestHost}",
        //      url, context.Request.Host.Host);
        //  throw new ProxyException("Circular request found")
        //      .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyForbiddenRequest)
        //      .WithFriendlyMessage("Access to local network protected resources is forbidden")
        //      .WithData("TargetUrl", url)
        //      .WithData("RequestHost", context.Request.Host.Host);
        //}


        var ignoreSsl = _configurationCacheService.GetSetting("ProxyRouter:IgnoreSslValidation")?.ToLower() == "true";
        // using 
        var httpClient = CreateHttpMessageInvoker(ignoreSsl);

        foreach (var ext in _extensions)
          await ext.OnBeforeForwardAsync(context, location);

        _logger.LogDebug("Forwarding started: {TargetUrl}", url);

        var stopwatch = Stopwatch.StartNew();

        var error = await _httpForwarder.SendAsync(
            context,
            url,
            httpClient,
            new ForwarderRequestConfig
            {
              ActivityTimeout = TimeSpan.FromSeconds(100),
              AllowResponseBuffering = true,
            },
            new CustomTransformer(url)).ConfigureAwait(false);

        stopwatch.Stop();

        foreach (var ext in _extensions)
          await ext.OnAfterForwardAsync(context, location, error, stopwatch.Elapsed);

        if (error != ForwarderError.None)
        {
          var errFeature = context.GetForwarderErrorFeature();
          _logger.LogWarning("Forwarding failed from {fromUrl} {toUrl}. Error={Error}, Duration={Elapsed}ms",
              context.Request.GetDisplayUrl(), url, error, stopwatch.ElapsedMilliseconds);

          throw new ProxyException($"Forwarding failed from ({context.Request.Method} {context.Request.GetDisplayUrl()}) to url ({url}). Reason: {error}", errFeature?.Exception)
              .WithStatusCode(EleonsoftStatusCodes.Proxy.ForwardingFailed)
              .WithData("ForwarderError", errFeature?.Error)
              .WithFriendlyMessage($"Forwarding failed. URL: {url}");
        }

        _logger.LogDebug("Forwarding completed successfully. Target={Url}, Duration={Elapsed}ms",
            url, stopwatch.ElapsedMilliseconds);
      }
      finally
      {
      }
    }


    private static HttpMessageInvoker _httpMessageInvoker;

    private static HttpMessageInvoker CreateHttpMessageInvoker(bool ignoreSsl)
    {
      if (_httpMessageInvoker != null)
      {
        return _httpMessageInvoker;
      }

      var handler = new SocketsHttpHandler
      {
        UseProxy = false,
        AllowAutoRedirect = false, // ⬅️ IMPORTANT: don’t follow 302s, let them hit the browser
        UseCookies = false,         // ⬅️ IMPORTANT: don’t swallow cookies in the proxy
        AutomaticDecompression = DecompressionMethods.None,
        ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
        MaxConnectionsPerServer = int.MaxValue, // Don't limit localhost connections
        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
      };

      if (ignoreSsl)
      {
        handler.SslOptions = new SslClientAuthenticationOptions
        {
          RemoteCertificateValidationCallback = (_, _, _, _) => true
        };
      }

      _httpMessageInvoker = new HttpMessageInvoker(handler);

      return _httpMessageInvoker;
    }
  }
}
