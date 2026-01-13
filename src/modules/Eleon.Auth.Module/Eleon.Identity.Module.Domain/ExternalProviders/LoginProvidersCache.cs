using Common.Module.Constants;
using Common.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;

namespace VPortal.Identity.Module.ExternalProviders
{
  public class LoginProvidersCache : ISingletonDependency
  {
    private readonly TenantSettingsCacheService cache;

    public LoginProvidersCache(TenantSettingsCacheService cache)
    {
      this.cache = cache;
    }

    public async ValueTask<IReadOnlyList<ExternalLoginProviderType>> GetTenantLoginProviders(Guid? tenantId)
    {
      //if (!ApplicationStateHelper.ApplicationInitializationCompleted)
      //{
      //    return new List<ExternalLoginProviderType>();
      //}

      var settings = await cache.GetTenantSettings(tenantId);
      return settings?.EnabledProviders ?? [];
    }
  }
}
