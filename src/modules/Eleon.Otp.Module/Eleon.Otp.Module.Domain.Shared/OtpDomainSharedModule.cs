using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating.Razor;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using VPortal.Otp.Module.Localization;
using VPortal.Otp.Module.Options;

namespace VPortal.Otp.Module;

[DependsOn(
    typeof(AbpValidationModule),
    typeof(AbpTextTemplatingRazorModule)
)]
public class OtpDomainSharedModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    var configuration = context.Services.GetConfiguration();

    PreConfigure<OtpOptions>(options =>
    {
      options.PreConfigure(configuration);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<OtpDomainSharedModule>();
    });

    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Add<OtpResource>("en")
              .AddBaseTypes(typeof(AbpValidationResource))
              .AddVirtualJson("/Localization/Otp");
    });

    Configure<AbpExceptionLocalizationOptions>(options =>
    {
      options.MapCodeNamespace("Otp", typeof(OtpResource));
    });
  }
}
