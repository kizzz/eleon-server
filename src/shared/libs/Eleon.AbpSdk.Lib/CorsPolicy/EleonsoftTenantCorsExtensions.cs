using Eleoncore.Module.TenantHostname;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eleoncore.SDK.TenantSettings
{
  public static class EleonsoftTenantCorsExtensions
  {
    public static IServiceCollection AddEleonsoftCorsWithHostnamePolicyProvider(this IServiceCollection services, Action<CorsPolicyBuilder> cfg = null)
    {
      if (services == null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      if (cfg != null)
      {
        services.AddSingleton<IEleonsoftCorsPolicyConfigurator>(new EleonsoftCorsPolicyConfigurator(cfg));
      }

      services.TryAdd(ServiceDescriptor.Transient<ICorsService, CorsService>());
      services.Replace(ServiceDescriptor.Transient<ICorsPolicyProvider, EleonsoftTenantHostnameCorsPolicyProvider>());

      return services;
    }
  }
}
