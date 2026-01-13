using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eleon.Common.Lib.UserToken;

public static class UserTokenExtensions
{
  public static IServiceCollection AddCurrentUserToken(this IServiceCollection services) {
    services.AddScoped<IUserTokenProvider, DefaultUserTokenProvider>();
    services.AddScoped<CurrentUserTokenMiddleware>();
    return services;
  }

  public static IApplicationBuilder UseCurrentUserToken(this IApplicationBuilder app)
  {
    app.UseMiddleware<CurrentUserTokenMiddleware>();
    return app;
  }
}
