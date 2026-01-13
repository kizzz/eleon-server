using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.GatewayManagement.Module;

[DependsOn(
    typeof(GatewayManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class GatewayManagementApplicationContractsModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<GatewayManagementApplicationContractsModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<GatewayManagementApplicationContractsModule>(validate: true);
    });
  }
}
