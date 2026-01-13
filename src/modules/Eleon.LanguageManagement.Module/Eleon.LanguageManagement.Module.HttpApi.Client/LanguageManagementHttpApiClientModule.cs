using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.LanguageManagement.Module;

[DependsOn(
    typeof(LanguageManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class LanguageManagementHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(LanguageManagementApplicationContractsModule).Assembly,
        LanguageManagementRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<LanguageManagementHttpApiClientModule>();
    });
  }
}
