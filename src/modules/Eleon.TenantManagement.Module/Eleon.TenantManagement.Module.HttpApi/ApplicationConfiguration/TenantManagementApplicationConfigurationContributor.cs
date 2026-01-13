using Common.Module.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.MultiTenancy;
using VPortal.TenantManagement.Module.TenantAppearance;

namespace VPortal.TenantManagement.Module.ApplicationConfiguration
{
  internal class TenantManagementApplicationConfigurationContributor : IApplicationConfigurationContributor
  {
    private const string LightLogoProperty = "LightTenantLogo";
    private const string DarkLogoProperty = "DarkTenantLogo";
    private const string LightIconProperty = "LightTenantIcon";
    private const string DarkIconProperty = "DarkTenantIcon";

    public async Task ContributeAsync(ApplicationConfigurationContributorContext context)
    {
      var appearanceService = context.ServiceProvider.GetRequiredService<ITenantAppearanceAppService>();
      var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
      var tenantCache = context.ServiceProvider.GetRequiredService<TenantCacheService>();
      var settings = await appearanceService.GetTenantAppearanceSettings();
      if (settings.LightIcon.NonEmpty())
      {
        context.ApplicationConfiguration.ExtraProperties[LightIconProperty] = settings.LightIcon;
      }

      if (settings.DarkIcon.NonEmpty())
      {
        context.ApplicationConfiguration.ExtraProperties[DarkIconProperty] = settings.DarkIcon;
      }

      if (settings.LightLogo.NonEmpty())
      {
        context.ApplicationConfiguration.ExtraProperties[LightLogoProperty] = settings.LightLogo;
      }

      if (settings.DarkLogo.NonEmpty())
      {
        context.ApplicationConfiguration.ExtraProperties[DarkLogoProperty] = settings.DarkLogo;
      }
      var tenant = (await tenantCache.GetTenantsAsync())
          .FirstOrDefault(t => t.Id == currentTenant.Id);

      context.ApplicationConfiguration.ExtraProperties["IsRoot"] = tenant == null ? true : tenant.IsRoot;
      context.ApplicationConfiguration.ExtraProperties["IsDefault"] = currentTenant.Id == null;
    }
  }
}
