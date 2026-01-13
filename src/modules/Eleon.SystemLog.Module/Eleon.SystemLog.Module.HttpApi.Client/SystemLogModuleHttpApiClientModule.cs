using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.DocMessageLog.Module;

[DependsOn(
    typeof(ModuleApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class SystemLogModuleHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(ModuleApplicationContractsModule).Assembly,
        SystemLogModuleRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<SystemLogModuleHttpApiClientModule>();
    });
  }
}
