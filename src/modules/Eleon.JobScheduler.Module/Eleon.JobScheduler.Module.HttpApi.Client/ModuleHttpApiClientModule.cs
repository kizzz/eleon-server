using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.JobScheduler.Module;

[DependsOn(
    typeof(JobSchedulerApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ModuleHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(JobSchedulerApplicationContractsModule).Assembly,
        JobSchedulerModuleRemoteServiceConsts.RemoteServiceName,
        asDefaultServices: true);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ModuleHttpApiClientModule>();
    });
  }
}
