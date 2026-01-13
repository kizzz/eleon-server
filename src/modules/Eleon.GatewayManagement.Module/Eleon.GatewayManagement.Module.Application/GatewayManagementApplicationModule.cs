using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.GatewayManagement.Module;

[DependsOn(
    typeof(GatewayManagementDomainModule),
    typeof(GatewayManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class GatewayManagementApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<GatewayManagementApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<GatewayManagementApplicationModule>(validate: true);
    });
  }
}
