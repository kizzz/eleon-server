using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(ProviderApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ProviderHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(ProviderApplicationContractsModule).Assembly,
        ProvidersRemoteServiceConsts.RemoteServiceName,
        asDefaultServices: true);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ProviderHttpApiClientModule>();
    });
  }
}
