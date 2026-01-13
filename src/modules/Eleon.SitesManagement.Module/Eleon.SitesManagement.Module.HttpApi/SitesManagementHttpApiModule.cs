using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using VPortal.SitesManagement.Module.ApplicationConfiguration;
using VPortal.SitesManagement.Module.Localization;

namespace VPortal.SitesManagement.Module;

[DependsOn(
    typeof(SitesManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class SitesManagementHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(SitesManagementHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<SitesManagementResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });

    Configure<AbpApplicationConfigurationOptions>(options =>
    {
      options.Contributors.Add(new SitesManagementApplicationConfigurationContributor());
    });
  }
}


