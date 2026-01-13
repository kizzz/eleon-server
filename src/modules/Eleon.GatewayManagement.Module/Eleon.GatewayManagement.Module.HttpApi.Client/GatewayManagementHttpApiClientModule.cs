using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.GatewayManagement.Module;

[DependsOn(
    typeof(GatewayManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class GatewayManagementHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(GatewayManagementApplicationContractsModule).Assembly,
        GatewayManagementRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<GatewayManagementHttpApiClientModule>();
    });

    PreConfigure<AbpHttpClientBuilderOptions>(options =>
    {
      options.ProxyClientHandlerActions.Add((remoteServiceName, services, httpClientHandler) =>
          {
          httpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
        });
    });
  }
}
