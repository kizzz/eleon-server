using EleonsoftAbp.EleonsoftIdentity.Sessions;
using Logging.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using VPortal.Identity.Module.Sessions;
// using VPortal.Identity.Module.Sessions;

namespace ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;

public interface ISessionAccessor
{
  FullSessionInformation Session { get; }
}

public class SessionInfoAccessor : ISessionAccessor
{
  private static readonly object _lock = new();
  private static readonly string SessionKey = "__FullSessionInformation__";
  private readonly IHttpContextAccessor _httpContextAccessor;

  public SessionInfoAccessor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public FullSessionInformation Session
  {
    get
    {
      var context = _httpContextAccessor.HttpContext;
      if (context == null)
        return GetDefault();

      if (context.Items.TryGetValue(SessionKey, out var value) && value is FullSessionInformation session)
        return session;

      return GetDefault();
    }
  }

  public void SetSession(FullSessionInformation session)
  {
    var context = _httpContextAccessor.HttpContext;
    if (context == null)
      return;

    lock (_lock)
    {
      context.Items[SessionKey] = session;
    }
  }

  public static FullSessionInformation GetDefault()
  {
    return new FullSessionInformation
    {
      SessionId = "Undefined",
      User = null,
      Tenant = null,
      Request = null
    };
  }
}

public class SessionMiddleware : IMiddleware
{
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger<SessionMiddleware> _logger;
  // private readonly IUserSession _userSession;
  private readonly ICurrentUser _currentUser;
  private readonly ICurrentTenant _currentTenant;
  private readonly ISessionAccessor _sessionAccessor;

  public SessionMiddleware(
      IServiceProvider serviceProvider,
      ILogger<SessionMiddleware> logger,
      // IUserSession userSession,
      ICurrentUser currentUser,
      ICurrentTenant currentTenant,
      ISessionAccessor sessionAccessor
      )
  {
    _serviceProvider = serviceProvider;
    _logger = logger;
    // _userSession = userSession;
    _currentUser = currentUser;
    _currentTenant = currentTenant;
    _sessionAccessor = sessionAccessor;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    _logger.LogDebug("Session middleware started");

    var sessionInfo = SessionInfoAccessor.GetDefault();

    try
    {
      var sessionId = ParseSessionHelper.GenerateSessionId(context); // await _userSession.GetSessionIdAsync();

      var request = context?.Request;

      var userAgent = request?.Headers["User-Agent"].FirstOrDefault();
      var deviceInfo = string.IsNullOrEmpty(userAgent) ? "Unknown" : ParseSessionHelper.GetDeviceInfo(userAgent).ToString();
      var xForwarderHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();

      sessionInfo = new FullSessionInformation()
      {
        SessionId = sessionId,
        User = new FullSessionInformation.UserInfo
        {
          IsAuthenticated = _currentUser.IsAuthenticated,
          Id = _currentUser.Id,
          UserName = _currentUser.UserName,
          Name = _currentUser.Name,
          SurName = _currentUser.SurName,
          PhoneNumber = _currentUser.PhoneNumber,
          PhoneNumberVerified = _currentUser.PhoneNumberVerified,
          Email = _currentUser.Email,
          EmailVerified = _currentUser.EmailVerified,
          TenantId = _currentUser.TenantId,
          Roles = _currentUser.Roles?.ToArray() ?? Array.Empty<string>(),
          Claims = _currentUser.GetAllClaims()?.ToArray() ?? Array.Empty<Claim>()
        },
        Tenant = new FullSessionInformation.TenantInfo
        {
          TenantId = _currentTenant.Id,
          Name = _currentTenant.Name
        },
        Request = new FullSessionInformation.RequestInfo
        {
          IpAddress = context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown",
          UserAgent = userAgent ?? "Unknown",
          ParsedDevice = deviceInfo,
          Host = request?.Host.Value ?? "Unknown",
          XForwardedFor = string.Join(" -> ", request?.Headers["X-Forwarded-For"].Where(x => !string.IsNullOrEmpty(x)).Select(x => x) ?? [])
        },
      };

      if (_sessionAccessor is SessionInfoAccessor accessor)
      {
        accessor.SetSession(sessionInfo);
        _logger.LogDebug("Session information set: {SessionInfo}", sessionInfo.ToString());
      }
      else
      {
        throw new NotImplementedException("Save session not implemented");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while processing the session middleware.");
    }

    using (LogContext.PushProperty("Session", _sessionAccessor.Session?.ToString()?.Replace('\n', ' ')))
    {
      try
      {
        await next(context);
      }
      finally
      {
        _logger.LogDebug("Session middleware finished");
      }
    }
  }
}


public static class SessionMiddlewareExtensions
{
  public static IServiceCollection AddSessionMiddleware(this IServiceCollection services)
  {
    services.AddTransient<SessionMiddleware>();
    services.AddSingleton<ISessionAccessor, SessionInfoAccessor>();
    return services;
  }

  public static IApplicationBuilder UseSessionMiddleware(this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<SessionMiddleware>();
  }
}
