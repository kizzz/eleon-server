using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Lifecycle.Feature.Module.Localization;

namespace VPortal.Lifecycle.Feature.Module;

[DependsOn(
    typeof(ModuleApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class ModuleHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(ModuleHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<LifecycleFeatureModuleResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
