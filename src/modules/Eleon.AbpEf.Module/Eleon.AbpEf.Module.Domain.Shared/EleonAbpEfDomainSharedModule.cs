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
public class EleonAbpEfDomainSharedModule : AbpModule
{

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<EleonAbpEfDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<EleonAbpResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/EleonAbp");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("EleonAbp", typeof(EleonAbpResource));
    });
  }
}
