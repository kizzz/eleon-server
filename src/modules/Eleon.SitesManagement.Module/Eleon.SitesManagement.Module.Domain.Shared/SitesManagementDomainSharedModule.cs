using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.SitesManagement.Module.Localization;

namespace VPortal.SitesManagement.Module;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class SitesManagementDomainSharedModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    SitesManagementModuleExtensionConfigurator.Configure();
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<SitesManagementDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<SitesManagementResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/SitesManagement");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("SitesManagement", typeof(SitesManagementResource));
    });
  }
}


