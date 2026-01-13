using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(TenantManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class TenantManagementHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(TenantManagementApplicationContractsModule).Assembly,
        TenantManagementRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<TenantManagementHttpApiClientModule>();
    });
  }
}
