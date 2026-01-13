using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Volo.Abp.Http.Client;
using VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants.MachineKey;

namespace GatewayManagement.Module.Proxies
{
  public static class GatewayServiceExtensions
  {
    public static IServiceCollection AddGatewayDynamicConfiguration(this IServiceCollection services)
        => services.Replace(ServiceDescriptor.Scoped<IRemoteServiceConfigurationProvider, GatewayRemoteServiceConfigurationProvider>());

    public static IServiceCollection AddGatewayHostMachineSecretsProvider(this IServiceCollection services)
        => services.AddTransient<IMachineSecretsProvider, GatewayHostMachineSecretsProvider>();
  }
}
