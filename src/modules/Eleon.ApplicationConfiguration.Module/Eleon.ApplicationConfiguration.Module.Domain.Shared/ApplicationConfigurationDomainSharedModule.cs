using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating.Razor;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.ApplicationConfiguration.Module.Localization;

namespace VPortal.ApplicationConfiguration.Module;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpTextTemplatingRazorModule)
)]
public class ApplicationConfigurationDomainSharedModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<ApplicationConfigurationDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<ApplicationConfigurationResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/ApplicationConfiguration");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("ApplicationConfiguration", typeof(ApplicationConfigurationResource));
    });
  }
}
