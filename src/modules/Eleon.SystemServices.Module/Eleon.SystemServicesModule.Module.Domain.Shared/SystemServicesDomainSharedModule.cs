using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.SystemServicesModule.Module.Localization;

namespace VPortal.SystemServicesModule.Module;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpLocalizationModule)
)]
public class SystemServicesDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<SystemServicesDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<SystemServicesModuleResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/SystemServicesModule");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("SystemServicesModule", typeof(SystemServicesModuleResource));
    });
  }
}

