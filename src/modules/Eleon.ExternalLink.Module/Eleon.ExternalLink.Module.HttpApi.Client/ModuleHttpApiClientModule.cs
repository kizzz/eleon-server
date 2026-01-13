using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.ExternalLink.Module;

[DependsOn(
    typeof(ModuleApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ModuleHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(ModuleApplicationContractsModule).Assembly,
        ExternalLinkRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ModuleHttpApiClientModule>();
    });
  }
}
