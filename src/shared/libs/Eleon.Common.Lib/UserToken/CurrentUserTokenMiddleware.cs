using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eleon.Common.Lib.UserToken;

public class CurrentUserTokenMiddleware : IMiddleware
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ILogger<CurrentUserTokenMiddleware> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly IUserTokenProvider _userTokenProvider;

  public CurrentUserTokenMiddleware(
    IHttpContextAccessor httpContextAccessor,
    ILogger<CurrentUserTokenMiddleware> logger,
    IServiceProvider serviceProvider,
    IUserTokenProvider userTokenProvider)
  {
    _httpContextAccessor = httpContextAccessor;
    _logger = logger;
    _serviceProvider = serviceProvider;
    _userTokenProvider = userTokenProvider;
  }


  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    //using (_userTokenProvider.Push(GetToken()))
    //{
    //  await next(context);
    //}

    _userTokenProvider.Token = GetToken();
    await next(context);
  }

  private string? GetToken()
  {
    try
    {
      var context = _httpContextAccessor.HttpContext;
      if (context == null)
      {
        return null;
      }

      var httpContextToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

      if (string.IsNullOrWhiteSpace(httpContextToken))
      {
        return null;
      }

      string token;
      if (httpContextToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      {
        token = httpContextToken.Substring("Bearer ".Length).Trim();
      }
      else
      {
        token = httpContextToken;
      }

      return token;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving token from HTTP context.");
      return null;
    }
  }
}
