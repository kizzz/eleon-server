using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Localization;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;
using Volo.Abp.Threading;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class CustomDynamicPermissionDefinitionStore : IDynamicPermissionDefinitionStore, ITransientDependency
  {
    protected IPermissionGroupDefinitionRecordRepository PermissionGroupRepository { get; }
    protected IPermissionDefinitionRecordRepository PermissionRepository { get; }
    protected IPermissionDefinitionSerializer PermissionDefinitionSerializer { get; }
    protected IDynamicPermissionDefinitionStoreInMemoryCache StoreCache { get; }
    protected IDistributedCache DistributedCache { get; }
    protected IAbpDistributedLock DistributedLock { get; }
    public PermissionManagementOptions PermissionManagementOptions { get; }
    protected AbpDistributedCacheOptions CacheOptions { get; }

    public CustomDynamicPermissionDefinitionStore(
        IPermissionGroupDefinitionRecordRepository permissionGroupRepository,
        IPermissionDefinitionRecordRepository permissionRepository,
        IPermissionDefinitionSerializer permissionDefinitionSerializer,
        IDynamicPermissionDefinitionStoreInMemoryCache storeCache,
        IDistributedCache distributedCache,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IOptions<PermissionManagementOptions> permissionManagementOptions,
        IAbpDistributedLock distributedLock)
    {
      PermissionGroupRepository = permissionGroupRepository;
      PermissionRepository = permissionRepository;
      PermissionDefinitionSerializer = permissionDefinitionSerializer;
      StoreCache = storeCache;
      DistributedCache = distributedCache;
      DistributedLock = distributedLock;
      PermissionManagementOptions = permissionManagementOptions.Value;
      CacheOptions = cacheOptions.Value;
    }

    public virtual async Task<PermissionDefinition> GetOrNullAsync(string name)
    {
      if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
      {
        return null;
      }

      using (await StoreCache.SyncSemaphore.LockAsync())
      {
        await EnsureCacheIsUptoDateAsync();
        return StoreCache.GetPermissionOrNull(name);
      }
    }

    public virtual async Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync()
    {
      if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
      {
        return Array.Empty<PermissionDefinition>();
      }

      using (await StoreCache.SyncSemaphore.LockAsync())
      {
        await EnsureCacheIsUptoDateAsync();
        return StoreCache.GetPermissions().ToImmutableList();
      }
    }

    public virtual async Task<IReadOnlyList<PermissionGroupDefinition>> GetGroupsAsync()
    {
      if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
      {
        return Array.Empty<PermissionGroupDefinition>();
      }

      using (await StoreCache.SyncSemaphore.LockAsync())
      {
        await EnsureCacheIsUptoDateAsync();
        return StoreCache.GetGroups().ToImmutableList();
      }
    }

    protected virtual async Task EnsureCacheIsUptoDateAsync()
    {
      if (StoreCache.LastCheckTime.HasValue &&
          DateTime.Now.Subtract(StoreCache.LastCheckTime.Value).TotalSeconds < 30)
      {
        /* We get the latest permission with a small delay for optimization */
        return;
      }

      var stampInDistributedCache = await GetOrSetStampInDistributedCache();

      if (stampInDistributedCache == StoreCache.CacheStamp)
      {
        StoreCache.LastCheckTime = DateTime.Now;
        return;
      }

      await UpdateInMemoryStoreCache();

      StoreCache.CacheStamp = stampInDistributedCache;
      StoreCache.LastCheckTime = DateTime.Now;
    }

    public async Task UpdateInMemoryCache()
    {
      await UpdateInMemoryStoreCache();
    }
    protected virtual async Task UpdateInMemoryStoreCache()
    {
      var permissionGroupRecords = (await PermissionGroupRepository.GetListAsync()).DistinctBy(x => x.Name).ToList();
      var permissionRecords = (await PermissionRepository.GetListAsync()).DistinctBy(x => x.Name).ToList();

      try
      {
        await StoreCache.FillAsync(permissionGroupRecords, permissionRecords);
      }
      catch (Exception)
      {
      }
    }

    protected virtual async Task<string> GetOrSetStampInDistributedCache()
    {
      var cacheKey = GetCommonStampCacheKey();

      var stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
      if (stampInDistributedCache != null)
      {
        return stampInDistributedCache;
      }

      await using (var commonLockHandle = await DistributedLock
                       .TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(2)))
      {
        if (commonLockHandle == null)
        {
          /* This request will fail */
          throw new AbpException(
              "Could not acquire distributed lock for permission definition common stamp check!"
          );
        }

        stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
        if (stampInDistributedCache != null)
        {
          return stampInDistributedCache;
        }

        stampInDistributedCache = Guid.NewGuid().ToString();

        await DistributedCache.SetStringAsync(
            cacheKey,
            stampInDistributedCache,
            new DistributedCacheEntryOptions
            {
              SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
            }
        );
      }

      return stampInDistributedCache;
    }

    protected virtual string GetCommonStampCacheKey()
    {
      return $"{CacheOptions.KeyPrefix}_AbpInMemoryPermissionCacheStamp";
    }

    protected virtual string GetCommonDistributedLockKey()
    {
      return $"{CacheOptions.KeyPrefix}_Common_AbpPermissionUpdateLock";
    }
  }

  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  public class CustomDynamicPermissionDefinitionStoreInMemoryCache : IDynamicPermissionDefinitionStoreInMemoryCache
  {
    public string CacheStamp { get; set; }

    protected IDictionary<string, PermissionGroupDefinition> PermissionGroupDefinitions { get; }
    protected IDictionary<string, PermissionDefinition> PermissionDefinitions { get; }
    protected ISimpleStateCheckerSerializer StateCheckerSerializer { get; }
    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

    public SemaphoreSlim SyncSemaphore { get; } = new(1, 1);

    public DateTime? LastCheckTime { get; set; }

    public CustomDynamicPermissionDefinitionStoreInMemoryCache(
        ISimpleStateCheckerSerializer stateCheckerSerializer,
        ILocalizableStringSerializer localizableStringSerializer)
    {
      StateCheckerSerializer = stateCheckerSerializer;
      LocalizableStringSerializer = localizableStringSerializer;

      PermissionGroupDefinitions = new Dictionary<string, PermissionGroupDefinition>();
      PermissionDefinitions = new Dictionary<string, PermissionDefinition>();
    }

    public Task FillAsync(
        List<PermissionGroupDefinitionRecord> permissionGroupRecords,
        List<PermissionDefinitionRecord> permissionRecords)
    {
      PermissionGroupDefinitions.Clear();
      PermissionDefinitions.Clear();

      var context = new PermissionDefinitionContext(null);

      foreach (var permissionGroupRecord in permissionGroupRecords)
      {
        var permissionGroup = context.AddGroup(
            permissionGroupRecord.Name,
            permissionGroupRecord.DisplayName != null ? LocalizableStringSerializer.Deserialize(permissionGroupRecord.DisplayName) : null
        );

        PermissionGroupDefinitions[permissionGroup.Name] = permissionGroup;

        foreach (var property in permissionGroupRecord.ExtraProperties)
        {
          permissionGroup[property.Key] = property.Value;
        }

        var permissionRecordsInThisGroup = permissionRecords
            .Where(p => p.GroupName == permissionGroup.Name);

        foreach (var permissionRecord in permissionRecordsInThisGroup.Where(x => x.ParentName == null))
        {
          AddPermissionRecursively(permissionGroup, permissionRecord, permissionRecords);
        }
      }

      return Task.CompletedTask;
    }

    public PermissionDefinition GetPermissionOrNull(string name)
    {
      return PermissionDefinitions.GetOrDefault(name);
    }

    public IReadOnlyList<PermissionDefinition> GetPermissions()
    {
      return PermissionDefinitions.Values.ToList();
    }

    public IReadOnlyList<PermissionGroupDefinition> GetGroups()
    {
      return PermissionGroupDefinitions.Values.ToList();
    }

    private void AddPermissionRecursively(ICanAddChildPermission permissionContainer,
        PermissionDefinitionRecord permissionRecord,
        List<PermissionDefinitionRecord> allPermissionRecords)
    {
      var permission = permissionContainer.AddPermission(
          permissionRecord.Name,
          permissionRecord.DisplayName != null ? LocalizableStringSerializer.Deserialize(permissionRecord.DisplayName) : null,
          permissionRecord.MultiTenancySide,
          permissionRecord.IsEnabled
      );

      PermissionDefinitions[permission.Name] = permission;

      if (!permissionRecord.Providers.IsNullOrWhiteSpace())
      {
        permission.Providers.AddRange(permissionRecord.Providers.Split(','));
      }

      if (!permissionRecord.StateCheckers.IsNullOrWhiteSpace())
      {
        var checkers = StateCheckerSerializer
            .DeserializeArray(
                permissionRecord.StateCheckers,
                permission
            );
        permission.StateCheckers.AddRange(checkers);
      }

      foreach (var property in permissionRecord.ExtraProperties)
      {
        permission[property.Key] = property.Value;
      }

      foreach (var subPermission in allPermissionRecords.Where(p => p.ParentName == permissionRecord.Name))
      {
        AddPermissionRecursively(permission, subPermission, allPermissionRecords);
      }
    }
  }
}
