using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module;

[DependsOn(
    typeof(ProviderApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class ProviderHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(ProviderHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<StorageModuleResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
