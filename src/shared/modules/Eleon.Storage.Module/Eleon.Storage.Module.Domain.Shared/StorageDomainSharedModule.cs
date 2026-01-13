using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
//using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(AbpValidationModule))]
public class StorageDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<StorageDomainSharedModule>();
    });

    //Configure<AbpLocalizationOptions>(options =>
    //{
    //    options.Resources
    //        .Add<StorageModuleResource>("en")
    //        .AddBaseTypes(typeof(AbpValidationResource))
    //        .AddVirtualJson("/Localization/StorageModule");
    //});

    //Configure<AbpExceptionLocalizationOptions>(options =>
    //{
    //    options.MapCodeNamespace("StorageModule", typeof(StorageModuleResource));
    //});
  }
}
