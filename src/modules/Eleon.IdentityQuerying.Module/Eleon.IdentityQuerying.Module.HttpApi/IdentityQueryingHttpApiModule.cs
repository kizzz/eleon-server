using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(IdentityQueryingApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class IdentityQueryingHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(IdentityQueryingHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    //Configure<AbpLocalizationOptions>(options =>
    //{
    //  options.Resources
    //          .Get<HealthCheckModuleResource>()
    //          .AddBaseTypes(typeof(AbpUiResource));
    //});
  }
}
