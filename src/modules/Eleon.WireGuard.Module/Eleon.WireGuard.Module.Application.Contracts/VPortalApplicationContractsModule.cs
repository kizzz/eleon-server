using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
//using Volo.Abp.LanguageManagement;
//using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
//using Volo.Abp.TextTemplateManagement;
using Volo.Abp.TenantManagement;
//using Volo.Saas.Host;

namespace VPortal;

[DependsOn(
    typeof(VPortalDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpMultiTenancyModule),
    // [ABP-PRO]: typeof(SaasHostApplicationContractsModule),
    // [ABP-PRO]: typeof(AbpAuditLoggingApplicationContractsModule),
    //typeof(AbpIdentityServerApplicationContractsModule),
    typeof(AbpAccountApplicationContractsModule)
//typeof(LanguageManagementApplicationContractsModule),
// [ABP-PRO]: typeof(LeptonThemeManagementApplicationContractsModule)
// [ABP-PRO]: typeof(TextTemplateManagementApplicationContractsModule)
)]
public class VPortalApplicationContractsModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    VPortalDtoExtensions.Configure();
  }
}
