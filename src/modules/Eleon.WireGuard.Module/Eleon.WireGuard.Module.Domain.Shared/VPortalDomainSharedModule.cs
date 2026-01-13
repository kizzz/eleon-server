using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.FeatureManagement;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.Identity;
// using Volo.Abp.IdentityServer;
//using Volo.Abp.LanguageManagement;
//using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
//using Volo.Abp.TextTemplateManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.Localization;

namespace VPortal;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpIdentityDomainSharedModule),
    // typeof(AbpIdentityServerDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    // [ABP-PRO]: typeof(LanguageManagementDomainSharedModule),
    typeof(AbpTenantManagementDomainSharedModule),
    // [ABP-PRO]: typeof(TextTemplateManagementDomainSharedModule),
    // [ABP-PRO]: typeof(LeptonThemeManagementDomainSharedModule),
    typeof(AbpGlobalFeaturesModule),
    typeof(BlobStoringDatabaseDomainSharedModule)
    )]
public class VPortalDomainSharedModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    VPortalGlobalFeatureConfigurator.Configure();
    VPortalModuleExtensionConfigurator.Configure();
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<VPortalDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<VPortalResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/VPortal");

      options.DefaultResourceType = typeof(VPortalResource);
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("VPortal", typeof(VPortalResource));
    });
  }
}
