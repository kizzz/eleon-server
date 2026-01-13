using TenantSettings.Module;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpTenantManagementDomainModule),
  typeof(AbpMultiTenancyModule),
    typeof(AbpAutoMapperModule),
    typeof(TenantSettingsModule),
    typeof(AbpMultiTenancyModule),
      typeof(AbpTenantManagementEntityFrameworkCoreModule),
      typeof(AbpIdentityEntityFrameworkCoreModule)
)]
public class IdentityQueryingDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();
    context.Services.AddAutoMapperObjectMapper<IdentityQueryingDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<IdentityQueryingDomainModule>(validate: true);
    });
  }
}
