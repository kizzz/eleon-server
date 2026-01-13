using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.Collaboration.Feature.Module.Localization;

namespace VPortal.Collaboration.Feature.Module;

[DependsOn(
    typeof(CollaborationApplicationContractsModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CollaborationHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(CollaborationHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<CollaborationResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });
  }
}
