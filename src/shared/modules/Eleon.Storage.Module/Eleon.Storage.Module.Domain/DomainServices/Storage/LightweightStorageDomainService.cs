using Common.Module.Helpers;
using Common.Module.Keys;
using Logging.Module;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Storage.Module.LightweightStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;

namespace Storage.Module.DomainServices
{

  public class LightweightStorageDomainService : DomainService
  {
    private readonly IVportalLogger<LightweightStorageDomainService> logger;
    private readonly StorageDomainService storageDomainService;
    private readonly StorageProviderOptionsManager _storageProviderOptionsManager;
    private readonly IDistributedCache<LightweightStorageItem, LightweightStorageKey> blobCache;

    public LightweightStorageDomainService(
        IVportalLogger<LightweightStorageDomainService> logger,
        StorageDomainService storageDomainService,
        StorageProviderOptionsManager storageProviderOptionsManager,
        IDistributedCache<LightweightStorageItem, LightweightStorageKey> blobCache)
    {
      this.logger = logger;
      this.storageDomainService = storageDomainService;
      _storageProviderOptionsManager = storageProviderOptionsManager;
      this.blobCache = blobCache;
    }

    public async Task SaveLightweightItem(string settingsGroup, string blobName, string dataBase64, LightweightStorageOptions options = null)
    {
      try
      {
        options ??= new();

        var key = new LightweightStorageKey(settingsGroup, blobName);
        await blobCache.RemoveAsync(key);

        if (dataBase64.IsNullOrWhiteSpace() || !Base64Helper.IsValidBase64(dataBase64))
        {
          throw new Exception("Invalid base64 provided or trying to set empty value.");
        }

        bool sizeLimitSet = options.MaxSize > 0;
        if (sizeLimitSet && Base64Helper.GetBase64BytesCount(dataBase64) > BytesHelper.GetBytes(options.MaxSize, options.MaxSizeUnit))
        {
          throw new Exception($"The file you are saving is too big (max {BytesHelper.ToString(options.MaxSize, options.MaxSizeUnit)})");
        }

        var storageItem = new LightweightStorageItem()
        {
          Base64 = dataBase64,
          Permissions = options.RequiredPermissions,
        };

        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          await storageDomainService.Save(settingsGroup, blobName, SerializeItem(storageItem), overrideExisting: true);
        }

        await blobCache.SetAsync(key, storageItem, new DistributedCacheEntryOptions
        {
          AbsoluteExpirationRelativeToNow = options.CacheExpiration,
        });
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<LightweightStorageItem> GetLightweightItem(LightweightStorageKey key)
    {
      LightweightStorageItem result = null;
      try
      {
        result = await blobCache.GetOrAddAsync(key, async () =>
        {
          using (StorageProviderOptionsManager.SetAmbientSettingsGroup(key.SettingsGroup))
          {
            var bytes = await storageDomainService.GetBytes(key.SettingsGroup, key.BlobName);
            return DeserializeItem(bytes);
          }
        });
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<LightweightStorageItem>> GetManyLightweightItems(List<LightweightStorageKey> keys)
    {
      List<LightweightStorageItem> result = null;
      try
      {
        var blobs = await blobCache.GetOrAddManyAsync(keys, async (notFoundKeys) =>
        {
          var base64s = new List<KeyValuePair<LightweightStorageKey, LightweightStorageItem>>();
          foreach (var notFoundKey in notFoundKeys)
          {
            using (StorageProviderOptionsManager.SetAmbientSettingsGroup(notFoundKey.SettingsGroup))
            {
              var bytes = await storageDomainService.GetBytes(notFoundKey.SettingsGroup, notFoundKey.BlobName);
              base64s.Add(KeyValuePair.Create(notFoundKey, DeserializeItem(bytes)));
            }
          }

          return base64s;
        });

        result = blobs.Select(x => x.Value).ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task Delete(string settingsGroup, string blobName)
    {
      try
      {
        var key = new LightweightStorageKey(settingsGroup, blobName);
        await blobCache.RemoveAsync(key);
        using (StorageProviderOptionsManager.SetAmbientSettingsGroup(settingsGroup))
        {
          await storageDomainService.Delete(settingsGroup, blobName);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private byte[] SerializeItem(LightweightStorageItem item)
        => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));

    private LightweightStorageItem DeserializeItem(byte[] bytes)
        => bytes == null ? null : JsonConvert.DeserializeObject<LightweightStorageItem>(UnicodeEncoding.UTF8.GetString(bytes));
  }
}
