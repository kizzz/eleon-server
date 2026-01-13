using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.ApplicationConfiguration.Module.Localization;

namespace VPortal.ApplicationConfiguration.Module;

[DependsOn(
    typeof(ApplicationConfigurationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class ApplicationConfigurationHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(ApplicationConfigurationHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<ApplicationConfigurationResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
