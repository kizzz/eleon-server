using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using ProxyRouter.Minimal.HttpApi.ErrorHandling.Exceptions;
using ProxyRouter.Minimal.HttpApi.Helpers;
using ProxyRouter.Minimal.HttpApi.Models.Messages;
using ProxyRouter.Minimal.HttpApi.Services;
using ProxyRouter.Module.Services;

namespace ProxyRouter.Minimal.HttpApi
{
  public static class ProxyRouterMinimalExtensions
  {
    public static IServiceCollection AddProxyRouter(this IServiceCollection services)
    {
      services.AddTransient<ProxyRouterService>();
      services.AddTransient<ConfigurationCacheService>();
      services.AddReverseProxy();

      services.AddSingleton<ILocationProvider, ConfigurationLocationProvider>();
      services.AddHttpForwarder();
      return services;
    }

    public static IApplicationBuilder UseProxyRouter(this IApplicationBuilder app)
    {
      var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
      if (!configuration.GetValue<bool>("ProxyRouter:Enabled"))
      {
        return app;
      }

      var pathes = configuration.GetSection("ProxyRouter:SystemPathes").Get<string[]>() ?? Array.Empty<string>();
      app.Use(async (context, next) =>
      {
        try
        {
          string route = context.Request.Path + context.Request.QueryString;

          //var moduleSettingsService = context.RequestServices.GetRequiredService<IModuleSettingsService>();
          var proxyRouterService = context.RequestServices.GetRequiredService<ProxyRouterService>();
          var locationProvider = context.RequestServices.GetRequiredService<ILocationProvider>();
          var configurationCacheService = context.RequestServices.GetRequiredService<ConfigurationCacheService>();

          // Handle the specific route "/clear-cache"
          if (route == "/clear-cache")
          {
            //moduleSettingsService.Clear();
            locationProvider.Clear();
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("Cache cleared!");
            return;
          }

          if (configurationCacheService.GetSetting("ProxyRouter:ForwardEmptyPath")?.ToLower() == "false" && (string.IsNullOrEmpty(route) || route == "/"))
          {
            await next();
            return;
          }

          if (pathes.Any(p => route.StartsWith(p, StringComparison.OrdinalIgnoreCase))) // POST Forbidden for demo
          {
            var demoDomains = configurationCacheService.GetSettingArray("ProxyRouter:DemoDomains");
            if (demoDomains.Contains(context.Request.Host.Host) && context.Request.Method != HttpMethods.Get && route.StartsWith("/api"))
            {
              throw new ProxyException("POST to API are forbidden for DEMO mode")
                  .WithData("DemoDomains", demoDomains)
                  .WithData("CurrentDomain", context.Request.Host.Host)
                  .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyForbiddenRequest)
                  .WithFriendlyMessage("For demo purposes, all POST request to API are forbidden");
            }
            await next();
            return;
          }

          var locations = new List<Location>();
          try
          {
            locations = await locationProvider.GetAsync();
          }
          catch (Exception ex)
          {
            throw new ProxyException("Failed to recieve locations", ex)
            .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyConfigurationError)
            .WithFriendlyMessage("Failed to recieve proxy configuration");
          }

          Location? location = null;
          if (context.Request.Path == "/proxy/request" && context.Request.Query.TryGetValue("url", out var requestUrl))
          {
            var url = requestUrl.FirstOrDefault();
            location = new Location
            {
              ResourceId = string.Empty,
              Path = string.Empty,
              DefaultRedirect = "/",
              CheckedPath = context.Request.Path,
              SourceUrl = requestUrl.ToString(),
              RemotePath = "",
              Type = LocationType.Other,
            };
          }
          else
          {
            location = LocationHandlingHelper.SelectLocation(locations, route);
          }

          if (location != null)
          {
            await proxyRouterService.HandleRequest(context, location);
          }
          else
          {
            await next();
          }

          return;
        }
        catch (ProxyException prEx)
        {
          prEx.RequestedRoute = context.Request.GetEncodedPathAndQuery();
          prEx.WithData("IgnoredPathes", pathes);
          throw;
        }
        catch (Exception ex)
        {
          var logger = context.RequestServices.GetRequiredService<ILogger<ProxyRouterService>>();
          logger.LogError(ex, "An unexpected error has occured in proxy middleware. Message: {message}", ex.Message);

          ex
              .WithData("IgnoredPathes", pathes)
              .WithStatusCode(EleonsoftStatusCodes.Proxy.ProxyInternalError)
              .WithFriendlyMessage("An unexpected proxy error occured.");

          throw;
        }
      });

      return app;
    }
  }
}
