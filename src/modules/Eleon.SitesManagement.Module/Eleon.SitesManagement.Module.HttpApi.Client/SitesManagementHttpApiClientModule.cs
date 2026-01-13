using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.SitesManagement.Module;

[DependsOn(
    typeof(SitesManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class SitesManagementHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(SitesManagementApplicationContractsModule).Assembly,
        SitesManagementRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<SitesManagementHttpApiClientModule>();
    });
  }
}

