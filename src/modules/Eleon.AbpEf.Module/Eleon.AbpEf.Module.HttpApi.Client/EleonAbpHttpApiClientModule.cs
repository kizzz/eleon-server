using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using VPortal.TenantManagement.Module;

namespace EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(EleonAbpEfApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class EleonAbpHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(EleonAbpEfApplicationContractsModule).Assembly,
        EleonAbpRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<EleonAbpHttpApiClientModule>();
    });
  }
}
