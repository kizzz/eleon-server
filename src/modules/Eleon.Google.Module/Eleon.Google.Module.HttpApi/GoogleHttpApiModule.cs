using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Google.Module.Localization;

namespace VPortal.Google.Module;

[DependsOn(
    typeof(GoogleApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class GoogleHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(GoogleHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<GoogleResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
