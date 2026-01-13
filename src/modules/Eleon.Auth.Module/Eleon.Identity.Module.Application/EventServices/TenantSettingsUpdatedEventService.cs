using ExternalLogin.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.Identity.Module.EventServices
{
  public class TenantSettingsUpdatedEventService :
      IDistributedEventHandler<TenantSettingsUpdatedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<TenantSettingsUpdatedEventService> logger;
    private readonly TenantSettingsCacheService cache;
    private readonly IOptionsMonitor<OpenIdConnectOptions> oidcOptions;

    public TenantSettingsUpdatedEventService(
        IVportalLogger<TenantSettingsUpdatedEventService> logger,
        TenantSettingsCacheService cache,
        IOptionsMonitor<OpenIdConnectOptions> oidcOptions)
    {
      this.logger = logger;
      this.cache = cache;
      this.oidcOptions = oidcOptions;
    }

    public async Task HandleEventAsync(TenantSettingsUpdatedMsg eventData)
    {
      try
      {
        await cache.UpdateCache();

        if (oidcOptions is OpenIdConnectOptionsProvider oidcOptionsProvider)
        {
          oidcOptionsProvider.ClearCache();
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
