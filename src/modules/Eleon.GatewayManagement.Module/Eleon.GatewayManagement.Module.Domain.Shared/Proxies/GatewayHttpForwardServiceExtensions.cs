using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;

namespace GatewayManagement.Module.Proxies
{
  public static class GatewayHttpForwardServiceExtensions
  {
    public static void AddGatewayHttpForwardHandler(this AbpHttpClientBuilderOptions options)
    {
      options.ProxyClientBuildActions.Add((remoteServiceName, clientBuilder) =>
      {
        clientBuilder.AddHttpMessageHandler<GatewayHttpForwardDelegatingHandler>();
      });
    }
  }
}
