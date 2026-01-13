using EleoncoreMultiTenancy.Module.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EleoncoreMultiTenancy.Module;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class EleoncoreMultiTenancyDomainSharedModule : AbpModule
{

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<EleoncoreMultiTenancyDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<EleoncoreMultiTenancyResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/EleoncoreMultiTenancy");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("EleoncoreMultiTenancy", typeof(EleoncoreMultiTenancyResource));
    });
  }
}
