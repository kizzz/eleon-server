using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace Authorization.Module.ClientIsolation
{
  public class NonAuthorizedClientIsolationMiddleware : IMiddleware, ITransientDependency
  {
    private readonly IAuthenticationSchemeProvider schemes;
    private readonly ClientIsolationMiddleware middleware;

    public NonAuthorizedClientIsolationMiddleware(
        IAuthenticationSchemeProvider schemes,
        ClientIsolationMiddleware middleware)
    {
      this.schemes = schemes;
      this.middleware = middleware;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      bool isAuthenticated = await CheckAuthentication(context);
      if (isAuthenticated)
      {
        await next(context);
      }
      else
      {
        await middleware.InvokeAsync(context, next);
      }
    }

    private async Task<bool> CheckAuthentication(HttpContext context)
    {
      var defaultAuthenticate = await schemes.GetDefaultAuthenticateSchemeAsync();
      if (defaultAuthenticate != null)
      {
        var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
        if (result?.Principal != null)
        {
          return true;
        }
      }

      return false;
    }
  }
}
