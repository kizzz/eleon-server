using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using VPortal.TenantManagement.Module;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(IdentityQueryingDomainModule),
    typeof(IdentityQueryingApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class IdentityQueryingApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<IdentityQueryingApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<IdentityQueryingApplicationModule>(validate: true);
    });
  }
}
