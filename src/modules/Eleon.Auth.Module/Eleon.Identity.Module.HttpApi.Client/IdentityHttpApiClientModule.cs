using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Identity.Module;

[DependsOn(
    typeof(IdentityApplicationContractsModule),
    typeof(AbpHttpClientIdentityModelModule),
    typeof(AbpHttpClientModule))]
public class IdentityHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(IdentityApplicationContractsModule).Assembly,
        IdentityRemoteServiceConsts.RemoteServiceName,
        asDefaultServices: false);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<IdentityHttpApiClientModule>();
    });
  }
}
