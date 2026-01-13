using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using VPortal.TenantManagement.Module.ApplicationConfiguration;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(TenantManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpIdentityHttpApiModule)
)]
public class TenantManagementHttpApiModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<IMvcBuilder>(mvcBuilder =>
    {
      mvcBuilder.AddApplicationPartIfNotExists(typeof(TenantManagementHttpApiModule).Assembly);
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Configure<AbpLocalizationOptions>(options =>
    {
      options.Resources
              .Get<TenantManagementResource>()
              .AddBaseTypes(typeof(AbpUiResource));
    });

    Configure<AbpApplicationConfigurationOptions>(options =>
    {
      options.Contributors.Add(new TenantManagementApplicationConfigurationContributor());
    });
  }
}
