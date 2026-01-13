using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class TenantManagementDomainSharedModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    TenantManagementModuleExtensionConfigurator.Configure();
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<TenantManagementDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<TenantManagementResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/TenantManagement");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("TenantManagement", typeof(TenantManagementResource));
    });
  }
}
