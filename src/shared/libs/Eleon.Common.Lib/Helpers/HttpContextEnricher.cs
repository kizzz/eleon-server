using EleonsoftSdk.modules.Helpers.Module;
using Microsoft.AspNetCore.Http;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EleonsoftAbp.Auth;
using System.Security.Principal;
using Eleon.Logging.Lib.SystemLog.Contracts;

namespace Eleon.Logging.Lib.SystemLog.Enrichers;

public sealed class HttpContextEnricher : ISystemLogEnricher
{
  public void Enrich(Dictionary<string, string> entry)
  {
    var httpContextAccessor = StaticServicesAccessor.GetService<IHttpContextAccessor>();
    var httpContext = httpContextAccessor?.HttpContext;

    if (httpContext == null)
    {
      return;
    }

    entry.AddIfNotExists("CorrelationId", httpContext.TraceIdentifier);
    entry.AddIfNotExists("RequestUrl", $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}");
    entry.AddIfNotExists("RequestMethod", httpContext.Request.Method);
    entry.AddIfNotExists("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");



    var apiKeyId = httpContext.User.GetApiKeyId();
    var userId = httpContext.User.GetUserId();
    if (!string.IsNullOrEmpty(apiKeyId))
    {
      var apiKeyName = httpContext.User.GetApiKeyName();
      entry.AddIfNotExists("InitiatorId", apiKeyId);
      entry.AddIfNotExists("InitiatorName", string.IsNullOrEmpty(apiKeyName) ? "ApiKey" : apiKeyName);
      entry.AddIfNotExists("InitiatorType", "ApiKey");
    }
    else if (!string.IsNullOrEmpty(userId))
    {
      var userName = httpContext.User.Identity?.Name;
      entry.AddIfNotExists("InitiatorId", userId);
      entry.AddIfNotExists("InitiatorName", string.IsNullOrEmpty(userName) ? "User" : userName);
      entry.AddIfNotExists("InitiatorType", "User");
    }
  }
}
