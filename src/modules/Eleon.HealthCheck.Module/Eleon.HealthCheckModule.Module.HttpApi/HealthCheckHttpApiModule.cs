using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.HealthCheckModule.Module.Localization;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(HealthCheckApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class HealthCheckHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(HealthCheckHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<HealthCheckModuleResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
