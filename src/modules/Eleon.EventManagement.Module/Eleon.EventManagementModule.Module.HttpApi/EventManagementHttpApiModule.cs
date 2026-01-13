using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.EventManagementModule.Module.Localization;

namespace VPortal.EventManagementModule.Module;

[DependsOn(
    typeof(EventManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class EventManagementHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(EventManagementHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<EventManagementModuleResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
