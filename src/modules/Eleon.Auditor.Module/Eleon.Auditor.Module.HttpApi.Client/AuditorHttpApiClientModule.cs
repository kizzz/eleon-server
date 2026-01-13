using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Auditing;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Auditor.Module;

[DependsOn(
    typeof(ModuleApplicationContractsModule),
    typeof(AbpHttpClientModule),
    typeof(AbpAuditingModule))]
public class AuditorHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(ModuleApplicationContractsModule).Assembly,
        AuditorRemoteServiceConsts.RemoteServiceName,
        asDefaultServices: true);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<AuditorHttpApiClientModule>();
    });
  }
}
