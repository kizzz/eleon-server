using Localization.Resources.AbpUi;
using Volo.Abp.Identity;
//using Volo.Abp.LanguageManagement;
//using Volo.Abp.LeptonTheme;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.CmsKit;
//using Volo.Abp.TextTemplateManagement;
//using Volo.Saas.Host;
using VPortal.Localization;

namespace VPortal;

[DependsOn(
    typeof(VPortalApplicationContractsModule),
    typeof(AbpIdentityHttpApiModule),
    //typeof(AbpPermissionManagementHttpApiModule),
    //typeof(AbpFeatureManagementHttpApiModule),
    //typeof(AbpSettingManagementHttpApiModule),
    // [ABP-PRO]: typeof(AbpAuditLoggingHttpApiModule),
    //typeof(AbpIdentityServerHttpApiModule),
    //typeof(AbpAccountHttpApiModule),
    // [ABP-PRO]: typeof(LanguageManagementHttpApiModule),
    // [ABP-PRO]: typeof(SaasHostHttpApiModule),
    //typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpMultiTenancyModule),
    typeof(CmsKitHttpApiModule)
    //typeof(CoreInfrastructureEntityFrameworkCoreModule)
    // [ABP-PRO]: typeof(LeptonThemeManagementHttpApiModule)
    // [ABP-PRO]: typeof(TextTemplateManagementHttpApiModule)
    )]
public class VPortalHttpApiModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    ConfigureLocalization();
  }

  private void ConfigureLocalization()
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<VPortalResource>()
              .AddBaseTypes(
                  typeof(AbpUiResource)
              );
    });
  }
}
