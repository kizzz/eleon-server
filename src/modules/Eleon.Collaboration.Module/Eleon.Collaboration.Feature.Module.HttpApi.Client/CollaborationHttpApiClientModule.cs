using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Collaboration.Feature.Module;

[DependsOn(
    typeof(CollaborationApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class CollaborationHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(CollaborationApplicationContractsModule).Assembly,
        CollaborationRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<CollaborationHttpApiClientModule>();
    });
  }
}
