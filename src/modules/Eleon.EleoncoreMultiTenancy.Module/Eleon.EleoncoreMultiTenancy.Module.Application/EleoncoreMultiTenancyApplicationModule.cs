using EleoncoreMultiTenancy.Module;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace VPortal.EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(EleoncoreMultiTenancyDomainModule),
    typeof(EleoncoreMultiTenancyApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementApplicationModule)
    )]
public class EleoncoreMultiTenancyApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<EleoncoreMultiTenancyApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<EleoncoreMultiTenancyApplicationModule>(validate: true);
    });
  }
}
