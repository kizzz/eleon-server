using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Notificator.Module.Localization;

namespace VPortal.Notificator.Module;

[DependsOn(
    typeof(ModuleApplicationContractsModule),
    typeof(AbpAspNetCoreSignalRModule),
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
              .Get<NotificatorResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
