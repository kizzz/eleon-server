using Microsoft.Extensions.DependencyInjection;
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
using Volo.Abp.VirtualFileSystem;
//using Volo.Saas.Host;
using Volo.CmsKit;

namespace VPortal;

[DependsOn(
    typeof(VPortalApplicationContractsModule),
    typeof(AbpIdentityHttpApiClientModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpTenantManagementHttpApiClientModule),
    typeof(AbpMultiTenancyModule)
// [ABP-PRO]: typeof(SaasHostHttpApiClientModule),
// [ABP-PRO]: typeof(AbpAuditLoggingHttpApiClientModule),
//typeof(AbpIdentityServerHttpApiClientModule),
//typeof(AbpAccountAdminHttpApiClientModule),
//typeof(AbpAccountPublicHttpApiClientModule),
//typeof(LanguageManagementHttpApiClientModule),
// [ABP-PRO]: typeof(LeptonThemeManagementHttpApiClientModule)
// [ABP-PRO]: typeof(TextTemplateManagementHttpApiClientModule)
)]
[DependsOn(typeof(CmsKitHttpApiClientModule))]
public class VPortalHttpApiClientModule : AbpModule
{
  public const string RemoteServiceName = "Default";

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(VPortalApplicationContractsModule).Assembly,
        RemoteServiceName,
        asDefaultServices: true);

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<VPortalHttpApiClientModule>();
    });
  }
}
