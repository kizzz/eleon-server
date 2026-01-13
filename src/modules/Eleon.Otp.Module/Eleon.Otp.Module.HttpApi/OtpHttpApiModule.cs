using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Otp.Module.Localization;

namespace VPortal.Otp.Module;

[DependsOn(
    typeof(OtpApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class OtpHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(OtpHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<OtpResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
