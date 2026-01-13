using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Lifecycle.Feature.Module;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(ModuleApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule))]
public class ModuleApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ModuleApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ModuleApplicationModule>(validate: false); // Disable validation to allow partial mappings
    });
  }
}
