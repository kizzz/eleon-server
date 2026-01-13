using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using VPortal.TenantManagement.Module;

namespace EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(EleoncoreMultiTenancyApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class EleoncoreMultiTenancyHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(EleoncoreMultiTenancyApplicationContractsModule).Assembly,
        EleoncoreMultiTenancyRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<EleoncoreMultiTenancyHttpApiClientModule>();
    });
  }
}
