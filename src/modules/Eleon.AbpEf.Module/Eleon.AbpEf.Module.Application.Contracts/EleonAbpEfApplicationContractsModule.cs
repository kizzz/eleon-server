using MassTransit;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(EleonAbpEfDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class EleonAbpEfApplicationContractsModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    base.ConfigureServices(context);
  }

}
