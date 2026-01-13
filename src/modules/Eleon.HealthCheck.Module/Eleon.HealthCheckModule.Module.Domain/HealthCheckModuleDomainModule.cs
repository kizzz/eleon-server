using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(HealthCheckDomainSharedModule))]
public class HealthCheckModuleDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<HealthCheckModuleDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<HealthCheckModuleDomainModule>(validate: true);
    });
  }
}

