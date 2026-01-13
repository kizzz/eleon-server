using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using VPortal.Storage.Remote.Application.Contracts;

namespace VPortal.Storage.Remote.HttpApi.Client;

[DependsOn(
    typeof(StorageRemoteApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class StorageRemoteHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(StorageRemoteApplicationContractsModule).Assembly,
        StorageRemoteServiceConsts.RemoteServiceName,
        asDefaultServices: true);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<StorageRemoteHttpApiClientModule>();
    });
  }
}
