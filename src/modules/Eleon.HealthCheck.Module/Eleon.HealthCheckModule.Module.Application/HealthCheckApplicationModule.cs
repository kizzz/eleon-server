using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(HealthCheckModuleDomainModule),
    typeof(HealthCheckApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class HealthCheckApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<HealthCheckApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<HealthCheckApplicationModule>(validate: true);
    });
  }
}
