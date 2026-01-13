using Common.Module.Extensions;
using Eleon.Storage.Lib.Constants;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
using Logging.Module;
using Microsoft.Extensions.Logging;
using MailKit.Search;
using MassTransit.Initializers;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.modules.Helpers.Module;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.VfsShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.PermissionManagement;
using VPortal.Storage.Module.Cache;
using VPortal.Storage.Module.DynamicOptions;
using VPortal.Storage.Module.Entities;
using VPortal.Storage.Module.Repositories;

namespace VPortal.Storage.Module.DomainServices
{
  public class StorageProviderDomainService : DomainService
  {
    private readonly IStorageProviderRepository repository;
    private readonly IStorageProviderTypeRepository storageProviderTypesRepository;
    private readonly IStorageProviderSettingTypeRepository storageProviderSettingTypeRepository;
    private readonly IVportalLogger<StorageProviderDomainService> logger;
    private readonly IDistributedCache<StorageProviderEntity, ProviderInTenantCacheKey> storageProvidersCache;
    private readonly ContainersCacheDomainService containersCache;
    private readonly IObjectMapper objectMapper;

    public StorageProviderDomainService(
        IStorageProviderRepository repository,
        IStorageProviderTypeRepository storageProviderTypeRepository,
        IStorageProviderSettingTypeRepository settingTypeRepository,
        IVportalLogger<StorageProviderDomainService> logger,
        IDistributedCache<StorageProviderEntity, ProviderInTenantCacheKey> storageProvidersCache,
        ContainersCacheDomainService containersCache,
        IObjectMapper objectMapper
    )
    {
      this.repository = repository;
      this.storageProviderTypesRepository = storageProviderTypeRepository;
      this.storageProviderSettingTypeRepository = settingTypeRepository;
      this.logger = logger;
      this.storageProvidersCache = storageProvidersCache;
      this.containersCache = containersCache;
      this.objectMapper = objectMapper;
    }

    public async Task<StorageProviderEntity> SaveStorageProvider(StorageProviderEntity entity)
    {
      StorageProviderEntity result = null;
      try
      {
        //try
        //{
        //  entity.IsTested = await TestStorageProvider(entity);
        //}
        //catch (Exception ex)
        //{
        //  logger.CaptureAndSuppress(ex);
        //  entity.IsTested = false;
        //}
        //
        //if (!entity.IsTested)
        //{
        //  entity.IsActive = false;
        //}

        var existing = await repository.GetAsync(entity.Id);

        existing.Name = entity.Name;
        existing.IsActive = entity.IsActive;
        existing.IsTested = true;

        var settingTypes = await storageProviderSettingTypeRepository.GetListByStorageProviderType(existing.StorageProviderTypeName);
        var settingTypesDict = settingTypes.ToDictionary(x => x.Key);
        if (StorageDomainConsts.PossibleSettings.TryGetValue(existing.StorageProviderTypeName, out var possibleSettingsDict))
        {
          foreach (var constSettingType in possibleSettingsDict)
          {

            settingTypesDict[constSettingType.Key] = constSettingType;
          }
        }

        foreach (var setting in entity.Settings)
        {
          if (!settingTypesDict.TryGetValue(setting.Key, out var settingType))
          {
            throw new Exception($"The setting '{setting.Key}' is not valid for storage provider type '{existing.StorageProviderTypeName}'.");
          }

          if (settingType.Required && string.IsNullOrEmpty(setting.Value))
          {
            throw new Exception($"The setting '{setting.Key}' is required and cannot be empty.");
          }
        }
        // Idempotency: check if values are already set (simplified check)
        var nameChanged = existing.Name != entity.Name;
        var activeChanged = existing.IsActive != entity.IsActive;
        var settingsCountChanged = existing.Settings.Count != entity.Settings.Count;
        var settingsChanged = settingsCountChanged ||
            existing.Settings.Any(s => !entity.Settings.Any(es => es.Key == s.Key && es.Value == s.Value)) ||
            entity.Settings.Any(s => !existing.Settings.Any(es => es.Key == s.Key && es.Value == s.Value));

        if (!nameChanged && !activeChanged && !settingsChanged)
        {
          logger.Log.LogInformation(
            "Storage provider {ProviderId} already has the same values. Treating as idempotent success.",
            entity.Id);
          result = existing;
        }
        else
        {
          existing.Name = entity.Name;
          existing.IsActive = entity.IsActive;
          existing.IsTested = true;
          existing.Settings = entity.Settings;

          try
          {
            var updatedEntity = await repository.UpdateAsync(existing, true);
            result = updatedEntity;
          }
          catch (AbpDbConcurrencyException ex)
          {
            logger.Log.LogWarning(
              ex,
              "Concurrency conflict while updating storage provider {ProviderId}. Waiting for desired state...",
              entity.Id);

            var resolved = await ConcurrencyExtensions.WaitForDesiredStateAsync(
              async () =>
              {
                var currentEntity = await repository.GetAsync(entity.Id);
                var isDesired =
                  currentEntity.Name == entity.Name &&
                  currentEntity.IsActive == entity.IsActive &&
                  currentEntity.Settings.Count == entity.Settings.Count;

                var details =
                  $"Name={currentEntity.Name},IsActive={currentEntity.IsActive},SettingsCount={currentEntity.Settings.Count}";

                return new ConcurrencyExtensions.ConcurrencyWaitResult<StorageProviderEntity>(isDesired, currentEntity, details);
              },
              logger.Log,
              "UpdateStorageProvider",
              entity.Id
            );

            result = resolved;
          }
        }

        await storageProvidersCache.RemoveAsync(new ProviderInTenantCacheKey(CurrentTenant.Id, existing.Id.ToString()));
        containersCache.RemoveCacheEntry(new ContainerInTenantCacheKey(CurrentTenant.Id, existing.Id.ToString()));
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<StorageProviderEntity> CreateAsync(string name, string typeName)
    {
      StorageProviderEntity result = null;
      try
      {
        var entity = new StorageProviderEntity(GuidGenerator.Create());
        entity.Name = name;

        if (string.IsNullOrEmpty(typeName))
        {
          throw new Exception("Storage provider type name must be provided.");
        }
        var storageProviderType = await storageProviderTypesRepository.GetByName(typeName);
        entity.StorageProviderTypeName = storageProviderType.Name;

        result = await repository.InsertAsync(entity, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<StorageProviderEntity> GetStorageProvider(string key)
    {
      StorageProviderEntity result = null;
      try
      {
        result = await storageProvidersCache.GetOrAddAsync(
            new ProviderInTenantCacheKey(CurrentTenant.Id, key),
            () => repository.GetAsync(Guid.Parse(key), includeDetails: true),
            () => new DistributedCacheEntryOptions
            {
              AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
            });
        var settingTypes = await storageProviderSettingTypeRepository.GetListByStorageProviderType(result.StorageProviderTypeName);
        var settingTypesDict = settingTypes.ToDictionary(x => x.Key);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<StorageProviderEntity>> GetStorageProvidersListByIds(List<Guid> ids)
    {
      List<StorageProviderEntity> result = null;
      try
      {
        var storageProviderTypes = (await storageProviderTypesRepository.GetListAsync()).ToDictionary(x => x.Name, x => x.Parent);

        result = await repository.GetStorageProvidersListByIds(ids);

        foreach (var item in result)
        {
          if (item.StorageProviderTypeName == null)
            throw new Exception($"Storage provider '{item.Name}' has no type defined.");

          List<string> types = [];
          var parentType = item.StorageProviderTypeName;

          while (!string.IsNullOrEmpty(parentType) && storageProviderTypes.TryGetValue(parentType, out var type))
          {
            types.Add(parentType);
            parentType = type;
          }

          item.FullType = string.Join(StorageDomainConsts.StorageProviderTypeSeparator, types.AsEnumerable().Reverse());
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
    public async Task<List<StorageProviderEntity>> GetStorageProvidersList(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null)
    {
      List<StorageProviderEntity> result = null;
      try
      {
        var storageProviderTypes = (await storageProviderTypesRepository.GetListAsync()).ToDictionary(x => x.Name, x => x.Parent);

        result = await repository.GetStorageProvidersList(
            sorting,
            maxResultCount,
            skipCount,
            searchQuery);

        foreach (var item in result)
        {
          if (item.StorageProviderTypeName == null)
            throw new Exception($"Storage provider '{item.Name}' has no type defined.");

          List<string> types = [];
          var parentType = item.StorageProviderTypeName;

          while (!string.IsNullOrEmpty(parentType) && storageProviderTypes.TryGetValue(parentType, out var type))
          {
            types.Add(parentType);
            parentType = type;
          }

          item.FullType = string.Join(StorageDomainConsts.StorageProviderTypeSeparator, types.AsEnumerable().Reverse());
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<Dictionary<string, List<StorageProviderSettingTypeEntity>>> GetPossibleSettings()
    {

      var result = new Dictionary<string, List<StorageProviderSettingTypeEntity>>();

      try
      {
        var allSettingTypes = await storageProviderSettingTypeRepository.GetListAsync(includeDetails: true);

        foreach (var group in allSettingTypes.GroupBy(x => x.StorageProviderTypeName))
        {
          result[group.Key] = group
              .Select(x => new StorageProviderSettingTypeEntity(x.Id)
              {
                StorageProviderTypeName = x.StorageProviderTypeName,
                Type = x.Type,
                Key = x.Key,
                DefaultValue = x.DefaultValue,
                Description = x.Description,
                Hidden = x.Hidden,
                Required = x.Required
              })
              .ToList();
        }

        var storageProviderTypes = await storageProviderTypesRepository.GetListAsync();

        foreach (var pair in StorageDomainConsts.PossibleSettings)
        {
          var storageTypeName = pair.Key;

          if (!result.TryGetValue(storageTypeName, out var list))
          {
            list = new List<StorageProviderSettingTypeEntity>();
            result[storageTypeName] = list;
          }

          foreach (var constSettingType in pair.Value)
          {
            if (list.Any(x => x.Key == constSettingType.Key))
              continue;

            list.Add(constSettingType);
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }


    public async Task<bool> RemoveStorageProvider(string storageProviderKey)
    {
      bool result = false;
      try
      {
        await storageProvidersCache.RemoveAsync(new ProviderInTenantCacheKey(CurrentTenant.Id, storageProviderKey));
        containersCache.RemoveCacheEntry(new ContainerInTenantCacheKey(CurrentTenant.Id, storageProviderKey));
        if (Guid.TryParse(storageProviderKey, out Guid id))
        {
          await repository.DeleteAsync(id, true);
        }

        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    //public async Task<bool> TestStorageProvider(StorageProviderEntity provider)
    //{
    //  bool result = false;
    //  try
    //  {
    //    string testBlobName = StorageDomainConsts.TestBlobNamePrefix + Guid.NewGuid().ToString();
    //    string testString = StorageDomainConsts.TestStringPrefix + Guid.NewGuid().ToString();
    //    byte[] testData = Encoding.UTF8.GetBytes(testString);
    //    string temporaryKey = StorageDomainConsts.TestSettingGroupPrefix + CurrentTenant.Id.ToString() + Guid.NewGuid().ToString();
    //    var providerDto = objectMapper.Map<StorageProviderEntity, StorageProviderDto>(provider);

    //    var vfsBlobProvider = VfsBlobProvidersFactory.Create(providerDto, CurrentTenant.Id);
    //    using var stream = new MemoryStream(testData);
    //    result = await vfsBlobProvider.TestAsync(new VfsTestArgs(temporaryKey, testBlobName, stream));
    //  }
    //  catch (Exception e)
    //  {
    //    logger.Capture(e);
    //  }

    //  return result;
    //}

    public async Task<List<StorageProviderTypeEntity>> GetStorageProviderTypesList()
    {
      List<StorageProviderTypeEntity> result = null;
      try
      {
        result = await storageProviderTypesRepository.GetListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      return result;
    }

    internal bool RemoveFromCache(string storageProviderKey)
    {
      bool result = false;
      try
      {
        storageProvidersCache.Remove(new ProviderInTenantCacheKey(CurrentTenant.Id, storageProviderKey));
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    internal bool AddToCache(string storageProviderKey, StorageProviderEntity entity)
    {
      bool result = false;
      try
      {
        _ = storageProvidersCache.GetOrAdd(
            new ProviderInTenantCacheKey(CurrentTenant.Id, storageProviderKey),
            () => entity,
            () => new DistributedCacheEntryOptions
            {
              AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
            });
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
