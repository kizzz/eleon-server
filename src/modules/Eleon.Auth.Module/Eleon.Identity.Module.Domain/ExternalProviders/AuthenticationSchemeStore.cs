using Common.Module.Constants;
using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace VPortal.Identity.Module.ExternalProviders
{
  public class AuthenticationSchemeStore : IAuthenticationSchemeStore, ITransientDependency
  {
    private static readonly Dictionary<ExternalLoginProviderType, string> DisplayNames = new()
        {
            { ExternalLoginProviderType.AzureEntra, "Microsoft" },
        };

    private readonly LoginProvidersCache cache;
    private readonly ICurrentTenant currentTenant;
    private readonly Dictionary<ExternalLoginProviderType, AuthenticationScheme> schemeCache = new();

    public AuthenticationSchemeStore(
        LoginProvidersCache cache,
        ICurrentTenant currentTenant)
    {
      this.cache = cache;
      this.currentTenant = currentTenant;
    }

    public async ValueTask<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemes()
    {
      var providers = await cache.GetTenantLoginProviders(currentTenant.Id);
      return providers
          .Select(s => schemeCache.GetOrAdd(s, CreateScheme))
          .ToList();
    }

    private AuthenticationScheme CreateScheme(ExternalLoginProviderType provider)
    {
      string providerName = Enum.GetName(provider);
      var builder = new AuthenticationSchemeBuilder(providerName)
      {
        DisplayName = DisplayNames.GetValueOrDefault(provider, providerName),
        HandlerType = typeof(OpenIdConnectHandler),
      };

      return builder.Build();
    }
  }
}
