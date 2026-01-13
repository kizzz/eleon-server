using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace ExternalLogin.Module
{
  public class NonAuthorizedTenantResolveMiddleware : IMiddleware, ITransientDependency
  {
    private readonly IAuthenticationSchemeProvider _schemes;
    private readonly ICurrentTenant currentTenant;
    private readonly IOptions<AbpAspNetCoreMultiTenancyOptions> multitenancyOptions;
    private readonly OpenIdConnectStateResolver oidcTenantResolver;
    private readonly MultiTenancyMiddleware _multiTenancyMiddleware;

    public NonAuthorizedTenantResolveMiddleware(
        IAuthenticationSchemeProvider schemes,
        ICurrentTenant currentTenant,
        IOptions<AbpAspNetCoreMultiTenancyOptions> options,
        OpenIdConnectStateResolver oidcTenantResolver,
        MultiTenancyMiddleware multiTenancyMiddleware)
    {

      if (schemes == null)
      {
        throw new ArgumentNullException(nameof(schemes));
      }

      _schemes = schemes;
      this.currentTenant = currentTenant;
      this.multitenancyOptions = options;
      this.oidcTenantResolver = oidcTenantResolver;
      _multiTenancyMiddleware = multiTenancyMiddleware;
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
        await _multiTenancyMiddleware.InvokeAsync(context, async (HttpContext context) =>
        {
          oidcTenantResolver.TrySetCurrentTenantFromOidcState(context);
          await next(context);
        });
      }
    }

    private async Task<bool> CheckAuthentication(HttpContext context)
    {
      var defaultAuthenticate = await _schemes.GetDefaultAuthenticateSchemeAsync();
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
