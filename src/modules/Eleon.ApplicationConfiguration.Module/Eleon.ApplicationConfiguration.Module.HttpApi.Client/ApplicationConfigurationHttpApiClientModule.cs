using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.ApplicationConfiguration.Module;

[DependsOn(
    typeof(ApplicationConfigurationApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ApplicationConfigurationHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(ApplicationConfigurationApplicationContractsModule).Assembly,
        ApplicationConfigurationRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ApplicationConfigurationHttpApiClientModule>();
    });
  }
}
