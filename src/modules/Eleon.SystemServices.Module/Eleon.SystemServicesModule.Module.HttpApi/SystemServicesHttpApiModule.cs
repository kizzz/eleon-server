using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.SystemServicesModule.Module.Localization;

namespace VPortal.SystemServicesModule.Module;

[DependsOn(
    typeof(SystemServicesApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class SystemServicesHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(SystemServicesHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<SystemServicesModuleResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}

