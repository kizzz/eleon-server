using EleonsoftSdk.modules.HealthCheck.Module.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.General;
public class EleonsoftHealthCheckMiddleware : IMiddleware
{
  private readonly ILogger<EleonsoftHealthCheckMiddleware> _logger;
  private readonly HealthCheckOptions _options;

  public EleonsoftHealthCheckMiddleware(
      ILogger<EleonsoftHealthCheckMiddleware> logger,
      IOptions<HealthCheckOptions> options)
  {
    _logger = logger;
    _options = options.Value;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    try
    {
      var path = context.Request.Path;

      // UI page - requires auth if enabled
      if (_options.UI.Enabled && path.StartsWithSegments(new PathString(_options.UI.Path)))
      {
        // Check if it's the restart endpoint
        if (path.StartsWithSegments(new PathString(_options.UI.Path + "/restart")))
        {
          // Restart endpoint: POST-only, requires auth, off by default
          if (!_options.RestartEnabled)
          {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
          }

          if (context.Request.Method != HttpMethods.Post)
          {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return;
          }

          // Check authorization if required
          if (_options.RestartRequiresAuth && !context.User.Identity?.IsAuthenticated == true)
          {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
          }

          await HealthCheckStaticPageHelper.HandleServiceRestart(context);
          return;
        }

        // Regular UI page
        if (path.Value == _options.UI.Path || path.Value == _options.UI.Path + "/")
        {
          await HealthCheckStaticPageHelper.HandleUIPageAsync(context);
          return;
        }
      }

      // Health status endpoints
      if (_options.Enabled)
      {
        if (path.StartsWithSegments(new PathString(_options.HealthStatusPath)))
        {
          var subPath = path.Value?.Substring(_options.HealthStatusPath.Length) ?? "";
          if (subPath == "" || subPath == "/")
          {
            await HealthCheckStaticPageHelper.HandleHealthCheckUrl(context, false);
            return;
          }

          if (subPath == "/health-status")
          {
            await HealthCheckStaticPageHelper.HandleHealthCheckUrl(context, true);
            return;
          }
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred in HealthCheckMiddleware.");
      throw;
    }

    await next(context);
  }
}
