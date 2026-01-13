using Common.EventBus.Module;
using Commons.Module.Messages.Features;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using VPortal.Storage.Module.Cache;
using VPortal.Storage.Module.DynamicOptions;

namespace VPortal.Storage.Module.DomainServices
{
  public class StorageDomainService : DomainService
  {
    private readonly IVportalLogger<StorageDomainService> logger;
    private readonly IBlobContainerFactory containerFactory;
    private readonly ContainersCacheDomainService containersCache;
    private readonly IDistributedEventBus _eventBus;
    private readonly StorageProviderOptionsManager storageProviderOptionsManager;

    public StorageDomainService(
        IVportalLogger<StorageDomainService> logger,
        IBlobContainerFactory containerFactory,
        ContainersCacheDomainService containersCache,
        IDistributedEventBus featureSettingsAppService,
        StorageProviderOptionsManager storageProviderOptionsManager)
    {
      this.logger = logger;
      this.containerFactory = containerFactory;
      this.containersCache = containersCache;
      this._eventBus = featureSettingsAppService;
      this.storageProviderOptionsManager = storageProviderOptionsManager;
    }

    public async Task<bool> Save(string settingsGroup, string blobName, byte[] data, bool overrideExisting = false, CancellationToken cancellationToken = default)
    {
      bool result = false;
      try
      {
        var container = await GetContainer(settingsGroup, cancellationToken);
        await container.SaveAsync(blobName, data, overrideExisting, cancellationToken);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<byte[]> GetBytes(string settingsGroup, string blobName, CancellationToken cancellationToken = default)
    {
      byte[] result = null;
      try
      {
        var container = await GetContainer(settingsGroup, cancellationToken);
        result = await container.GetAllBytesOrNullAsync(blobName, cancellationToken);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> Delete(string settingsGroup, string blobName, CancellationToken cancellationToken = default)
    {
      bool result = false;
      try
      {
        var container = await GetContainer(settingsGroup, cancellationToken);
        result = await container.DeleteAsync(blobName, cancellationToken);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<bool> Exists(string settingsGroup, string blobName, CancellationToken cancellationToken = default)
    {
      bool result = false;
      try
      {
        var container = await GetContainer(settingsGroup, cancellationToken);
        result = await container.ExistsAsync(blobName, cancellationToken);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    private IBlobContainer CreateDefaultContainer()
        => containerFactory.Create(StorageDomainConsts.DefaultContainerName);

    private async Task<IBlobContainer> GetContainer(string settingsGroup, CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrEmpty(settingsGroup))
      {
        return CreateDefaultContainer();
      }

      string storageProviderKey = await GetSetting(settingsGroup, cancellationToken);
      if (string.IsNullOrEmpty(storageProviderKey))
      {
        return CreateDefaultContainer();
      }

      // Get provider settings to compute hash for cache key
      var settingsHash = string.Empty;
      try
      {
        var maybeSettings = await storageProviderOptionsManager.GetCurrentStorageProviderSettings(settingsGroup);
        if (maybeSettings.HasValue && maybeSettings.Value.Value != null)
        {
          settingsHash = ContainerInTenantCacheKey.ComputeSettingsHash(maybeSettings.Value.Value);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
        // Continue with empty hash if settings retrieval fails
      }

      var cacheKey = new ContainerInTenantCacheKey(CurrentTenant.Id, storageProviderKey, settingsHash);
      var cachedContainer = containersCache.GetOrAddCacheEntry(cacheKey, CreateDefaultContainer);
      return cachedContainer;
    }

    private async Task<string> GetSetting(string group, CancellationToken cancellationToken = default)
    {
      var response = await _eventBus.RequestAsync<GetFeatureSettingResponseMsg>(new GetFeatureSettingMsg
      {
        TenantId = CurrentTenant.Id,
        Group = group,
        Key = StorageDomainConsts.StorageProviderSettingKey,
      });

      return response?.Value;
    }
  }
}
