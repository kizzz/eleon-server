using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Storage.Module.Localization;
using VPortal.Storage.Remote.Application;

namespace VPortal.Storage.Remote.HttpApi;

[DependsOn(
    typeof(StorageRemoteApplicationModule),
    typeof(AbpAspNetCoreMvcModule))]
public class StorageRemoteHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(StorageRemoteHttpApiModule).Assembly);
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
