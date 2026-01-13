using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
//using Volo.Abp.LanguageManagement;
//using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
//using Volo.Abp.TextTemplateManagement;
//using Volo.Saas.Host;

namespace VPortal;

[DependsOn(
    typeof(VPortalDomainModule),
    typeof(VPortalApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpMultiTenancyModule),
    // [ABP-PRO]: typeof(SaasHostApplicationModule),
    // [ABP-PRO]: typeof(AbpAuditLoggingApplicationModule),
    //typeof(AbpIdentityServerApplicationModule),
    typeof(AbpAccountApplicationModule)
    // [ABP-PRO]: typeof(LanguageManagementApplicationModule),
    // [ABP-PRO]: typeof(LeptonThemeManagementApplicationModule)
    // [ABP-PRO]: typeof(TextTemplateManagementApplicationModule)
    )]
[DependsOn(typeof(AbpAutoMapperModule))]
public class VPortalApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<VPortalApplicationModule>();
    });

    Configure<AbpDistributedEntityEventOptions>(options =>
    {

    });


  }
}
