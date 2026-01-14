using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.SystemServicesModule.Module;

[DependsOn(
    typeof(SystemServicesModuleDomainModule),
    typeof(SystemServicesApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class SystemServicesApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<SystemServicesApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<SystemServicesApplicationModule>(validate: true);
    });
  }
}

