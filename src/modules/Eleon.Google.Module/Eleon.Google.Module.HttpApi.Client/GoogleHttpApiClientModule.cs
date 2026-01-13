using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Google.Module;

[DependsOn(
    typeof(GoogleApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class GoogleHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(GoogleApplicationContractsModule).Assembly,
        GoogleRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<GoogleHttpApiClientModule>();
    });
  }
}
