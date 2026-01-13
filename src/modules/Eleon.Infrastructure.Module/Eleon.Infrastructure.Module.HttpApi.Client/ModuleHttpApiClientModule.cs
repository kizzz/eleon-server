using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Infrastructure.Module;

[DependsOn(
    typeof(ModuleApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ModuleHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(ModuleApplicationContractsModule).Assembly,
        InfrastructureRemoteServiceConsts.RemoteServiceName,
        asDefaultServices: true);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ModuleHttpApiClientModule>();
    });

  }
}
