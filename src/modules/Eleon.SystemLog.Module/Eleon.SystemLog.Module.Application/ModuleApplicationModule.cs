using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.DocMessageLog.Module;

[DependsOn(
    typeof(SystemLogDomainModule),
    typeof(ModuleApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class ModuleApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ModuleApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ModuleApplicationModule>(validate: true);
    });
  }
}
