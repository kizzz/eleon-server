using Common.EventBus.Module;
using Eleon.Logging.Lib.SystemLog.Logger;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedModule.modules.MultiTenancy.Module;
using TenantSettings.Module.Messaging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TenantSettings.Module.Cache
{
  public record TenantSettingsInfo(Guid? Id, string Name, bool IsActive);
  public class TenantSettingsCacheService : ISingletonDependency
  {
    private static readonly SemaphoreSlim CacheSemaphore = new(1);
    private readonly ICurrentTenant currentTenant;
    private readonly IOptions<TenantSettingsCacheOptions> options;
    private readonly IDistributedEventBus eventBus;
    private readonly TenantCacheService tenantCacheService;
    private readonly EleonMultiTenancyOptions _multiTenanacyOptions;
    private Dictionary<string, TenantSettingsCacheValue> settingsCache = null;
    private Dictionary<string, Guid?> tenantsByUrlsLookup = null;
    private List<string> applicationUrls = null;
    private List<Guid?> inactiveTenants = null;

    private bool initialized = false;

    private const string HostTenantKey = "host";

    public TenantSettingsCacheService(
        ICurrentTenant currentTenant,
        IOptions<TenantSettingsCacheOptions> options,
        IDistributedEventBus eventBus,
        TenantCacheService tenantCacheService,
        IOptions<EleonMultiTenancyOptions> multiTenanacyOptions)
    {
      this.currentTenant = currentTenant;
      this.options = options;
      this.eventBus = eventBus;
      this.tenantCacheService = tenantCacheService;
      _multiTenanacyOptions = multiTenanacyOptions.Value;
    }

    public async Task UpdateCache()
    {
      await InitCache(force: true);
    }

    public async Task<TenantSettingsInfo?> GetTenantByUrl(string url, bool fullCheck = false)
    {
      var tenant = _multiTenanacyOptions.TenantDomains.FirstOrDefault(kv => kv.Value.Contains(url));

      if (!tenant.Value.IsNullOrEmpty())
      {
        return Guid.TryParse(tenant.Key, out var tId) ? new TenantSettingsInfo(tId, string.Empty, true) : new TenantSettingsInfo(null, string.Empty, true);
      }

      await InitCache();
      var tenantSt = settingsCache.FirstOrDefault(t => t.Value.TenantSetting.Hostnames.FirstOrDefault(h => h.Url == url) != null);
      if (tenantSt.Value != null)
      {
        return GetTenantFromCacheValue(tenantSt.Key, tenantSt.Value);
      }

      if (fullCheck)
      {
        await UpdateCache();

        tenantSt = settingsCache.FirstOrDefault(t => t.Value.TenantSetting.Hostnames.FirstOrDefault(h => h.Url == url) != null);
        if (tenantSt.Value != null)
        {
          return GetTenantFromCacheValue(tenantSt.Key, tenantSt.Value);
        }
      }

      return null;
    }

    public async ValueTask<(bool found, Guid? tenantId)> GetTenantIdByUrl(string url, bool fullCheck = false)
    {
      var tenant = _multiTenanacyOptions.TenantDomains.FirstOrDefault(kv => kv.Value.Contains(url));

      if (!tenant.Value.IsNullOrEmpty())
      {
        return Guid.TryParse(tenant.Key, out var tId) ? (true, tId) : (true, null);
      }

      await InitCache();

      bool found = tenantsByUrlsLookup.TryGetValue(url, out var tenantId);
      if (!found && fullCheck)
      {
        await UpdateCache();
        found = tenantsByUrlsLookup.TryGetValue(url, out tenantId);
      }

      return (found, tenantId);
    }

    public async ValueTask<TenantSettingsCacheValue> GetTenantSettings(Guid? tenantId)
    {
      await InitCache();
      var value = settingsCache.GetValueOrDefault(GetTenantCacheKey(tenantId), null);

      if (value == null)
      {
        value = new TenantSettingsCacheValue();
        settingsCache.Add(GetTenantCacheKey(tenantId), value);
      }

      return value;
    }

    public async ValueTask<IReadOnlyList<string>> GetApplicationUrls()
    {
      await InitCache();
      return applicationUrls;
    }

    public async ValueTask<IReadOnlyList<Guid?>> GetInactiveTenants()
    {
      await InitCache();
      return inactiveTenants;
    }

    private TenantSettingsInfo GetTenantFromCacheValue(string tenantKey, TenantSettingsCacheValue cacheValue)
    {
      if (tenantKey == HostTenantKey)
      {
        return new TenantSettingsInfo(null, HostTenantKey, cacheValue.IsActive);
      }
      var tenantId = Guid.Parse(tenantKey);
      var tenant = tenantCacheService.GetTenants().FirstOrDefault(t => t.Id == tenantId);
      return new TenantSettingsInfo(tenantId, tenant?.Name ?? string.Empty, cacheValue.IsActive);
    }

    private async Task InitCache(bool force = false)
    {
      if (!force && initialized)
      {
        return;
      }

      await CacheSemaphore.WaitAsync();
      try
      {
        using (currentTenant.Change(null))
        {
          EleonsoftLog.Info("Initializing TenantSettingsCacheService cache");
          var request = new GetTenantSettingsMsg();
          var response = await eventBus.RequestAsync<TenantSettingsGotMsg>(request, 60);

          if (string.IsNullOrEmpty(response.SettingsJson))
          {
            throw new InvalidDataException("Tenant settings was null");
          }

          TenantSettingsCacheEto? data = null;
          try
          {
            data = JsonConvert.DeserializeObject<TenantSettingsCacheEto>(response.SettingsJson);
          }
          catch (Exception ex)
          {
            throw new InvalidDataException("Tenant settings deserialization failed", ex);
          }

          if (data == null)
            throw new InvalidDataException("Tenant settings was null");

          settingsCache = data.TenantSettings
              .DistinctBy(x => x.TenantId)
              .ToDictionary(x => GetTenantCacheKey(x.TenantId), x => CreateTenantSettingsCacheEntry(x, data));

          var hostnames = data.TenantSettings
              .SelectMany(x => x.Hostnames)
              .DistinctBy(x => x.Url).ToArray();
          tenantsByUrlsLookup = hostnames.ToDictionary(x => x.Url, x => x.TenantId);

          applicationUrls = hostnames.Select(x => x.Url).Concat(data.Cors).Concat(_multiTenanacyOptions.TenantDomains.Values.SelectMany(x => x)).ToList();

          inactiveTenants = data.TenantSettings
              .Where(x => x.Status != Common.Module.Constants.TenantStatus.Active)
              .Select(x => x.TenantId)
              .ToList();

          // await NotifyIdentityUrlsChanged();
          EleonsoftLog.Info("TenantSettingsCacheService cache initialized");

          initialized = true;
        }
      }
      catch (Exception ex)
      {
        EleonsoftLog.Error($"TenantSettingsCacheService cache initialization failed: {ex.Message}", ex);
        throw;
      }
      finally
      {
        CacheSemaphore.Release();
      }
    }

    private static TenantSettingsCacheValue CreateTenantSettingsCacheEntry(Models.TenantSetting setting, TenantSettingsCacheEto eto)
    {
      var isolationSettings = eto.UserIsolationSettings.Where(x => x.TenantId == setting.TenantId).ToList();
      var adminIds = setting.TenantId == null ? eto.HostAdminUsers : [];

      return new(setting, isolationSettings, adminIds);
    }

    private static string GetTenantCacheKey(Guid? tenantId) => tenantId?.ToString() ?? HostTenantKey;

    //private async Task NotifyIdentityUrlsChanged()
    //{
    //    if (!options.Value.NotifyIdentityUrlsChange)
    //    {
    //        return;
    //    }

    //    var hostnames = settingsCache.Select(x => x.Value).SelectMany(x => x.TenantSetting.Hostnames).ToList();
    //    var applicationUrls = hostnames
    //        .Select(x => (x.ApplicationType, x.Url))
    //        .GroupBy(x => x.ApplicationType)
    //        .Select(x => new IdentityUrlsEto()
    //        {
    //            AppType = x.Key,
    //            Urls = x.Select(x => x.Url).ToList(),
    //        })
    //        .ToList();

    //    await eventBus.PublishAsync(new IdentityUrlsChangedMsg()
    //    {
    //        AppUrls = applicationUrls,
    //    });
    //}
  }
}


