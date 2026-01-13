using MassTransit;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(EleoncoreMultiTenancyDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class EleoncoreMultiTenancyApplicationContractsModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    base.ConfigureServices(context);
  }

}
