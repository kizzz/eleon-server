using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.LanguageManagement.Module.Localization;

namespace VPortal.LanguageManagement.Module;

[DependsOn(
    typeof(LanguageManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class LanguageManagementHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(LanguageManagementHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<LanguageManagementResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
