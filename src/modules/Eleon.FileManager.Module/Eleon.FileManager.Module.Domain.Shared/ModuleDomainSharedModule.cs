using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.FileManager.Module.Localization;

namespace VPortal.FileManager.Module;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class ModuleDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ModuleDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<ModuleResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/FileManagerModule");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("FileManagerModule", typeof(ModuleResource));
    });
  }
}
