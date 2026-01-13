using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Json.SystemTextJson.Modifiers;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  [ExposeServices(typeof(IStaticPermissionSaver), IncludeDefaults = true, IncludeSelf = true)]
  public class CustomStaticSaver : StaticPermissionSaver, IStaticPermissionSaver
  {
    private readonly ITenantRepository tenantRepository;
    private readonly ILogger<CustomStaticSaver> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant currentTenant;

    public CustomStaticSaver(
        IStaticPermissionDefinitionStore staticStore,
        IPermissionGroupDefinitionRecordRepository permissionGroupRepository,
        IPermissionDefinitionRecordRepository permissionRepository,
        IPermissionDefinitionSerializer permissionSerializer,
        IDistributedCache cache,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IApplicationInfoAccessor applicationInfoAccessor,
        IAbpDistributedLock distributedLock,
        IOptions<AbpPermissionOptions> permissionOptions,
        ICancellationTokenProvider cancellationTokenProvider,
        IUnitOfWorkManager unitOfWorkManager,
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        ITenantRepository tenantRepository,
        IDistributedEventBus distributedEventBus,
        ILogger<CustomStaticSaver> logger) : base(staticStore, permissionGroupRepository, permissionRepository, permissionSerializer, cache, cacheOptions, applicationInfoAccessor, distributedLock, permissionOptions, cancellationTokenProvider, unitOfWorkManager, distributedEventBus)
    {
      _configuration = configuration;
      this.currentTenant = currentTenant;
      this.tenantRepository = tenantRepository;
      _logger = logger;
    }

    public async new Task SaveAsync()
    {
      var cancellationToken = default(CancellationToken);
      using (CancellationTokenProvider.Use(cancellationToken))
      {
        var tenants = await tenantRepository.GetListAsync(cancellationToken: cancellationToken);
        var tenantIds = tenants.Select(x => (Guid?)x.Id).Concat([null]).Distinct();
        foreach (var tenantId in tenantIds)
        {
          try
          {
            _logger.LogDebug("Saving static permissions for tenant {tenantId}", tenantId);
            using (currentTenant.Change(tenantId))
            {
              await SaveTenantAsync(cancellationToken);
            }
            _logger.LogDebug("Saving static permissions for tenant {tenantId} successfully", tenantId);
          }
          catch (Exception ex)
          {
            _logger.LogError(
                ex,
                "An error occurred while saving static permissions for tenant {TenantId}",
                tenantId
            );
          }
        }
      }
    }

    public async Task SaveTenantAsync(CancellationToken cancellationToken = default)
    {
      await using var applicationLockHandle = await DistributedLock.TryAcquireAsync(GetApplicationDistributedLockKey(), cancellationToken: cancellationToken);

      if (applicationLockHandle == null)
      {
        /* Another application instance is already doing it */
        return;
      }

      /* NOTE: This can be further optimized by using 4 cache values for:
       * Groups, permissions, deleted groups and deleted permissions.
       * But the code would be more complex. This is enough for now.
       */

      var cacheKey = GetApplicationHashCacheKey();
      var cachedHash = await Cache.GetStringAsync(cacheKey, cancellationToken);

      var (permissionGroupRecords, permissionRecords) = await PermissionSerializer.SerializeAsync(
          await StaticStore.GetGroupsAsync()
      );

      var currentHash = CalculateHash(
          permissionGroupRecords,
          permissionRecords,
          PermissionOptions.DeletedPermissionGroups,
          PermissionOptions.DeletedPermissions
      );

      if (!_configuration.GetValue("Permissions:SkipCacheCheck", false))
      {
        if (cachedHash == currentHash)
        {
          _logger.LogDebug("Current permissions hash is equels to cached");
          return;
        }
      }

      await using (var commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(5), cancellationToken))
      {
        if (commonLockHandle == null)
        {
          /* It will re-try */
          throw new AbpException("Could not acquire distributed lock for saving static permissions!");
        }

        using (var unitOfWork = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
          try
          {
            var hasChangesInGroups = await UpdateChangedPermissionGroupsAsync(permissionGroupRecords, cancellationToken);
            var hasChangesInPermissions = await UpdateChangedPermissionsAsync(permissionRecords, cancellationToken);

            if (hasChangesInGroups || hasChangesInPermissions)
            {
              await Cache.SetStringAsync(
                  GetCommonStampCacheKey(),
                  Guid.NewGuid().ToString(),
                  new DistributedCacheEntryOptions
                  {
                    SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
                  },
                  cancellationToken
              );
            }
          }
          catch
          {
            try
            {
              await unitOfWork.RollbackAsync(cancellationToken);
            }
            catch
            {
              /* ignored */
            }

            throw;
          }

          await unitOfWork.CompleteAsync(cancellationToken);
        }
      }

      await Cache.SetStringAsync(
          cacheKey,
          currentHash,
          new DistributedCacheEntryOptions
          {
            SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
          },
          cancellationToken
      );
    }

    private async Task<bool> UpdateChangedPermissionGroupsAsync(
        IEnumerable<PermissionGroupDefinitionRecord> permissionGroupRecords,
        CancellationToken cancellationToken = default)
    {
      var newRecords = new List<PermissionGroupDefinitionRecord>();
      var changedRecords = new List<PermissionGroupDefinitionRecord>();

      var permissionGroupRecordsInDatabase = (await PermissionGroupRepository.GetListAsync(cancellationToken: cancellationToken))
          .Where(x => x.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty) == string.Empty)
          .ToDictionary(x => x.Name);

      foreach (var permissionGroupRecord in permissionGroupRecords)
      {
        var permissionGroupRecordInDatabase =
            permissionGroupRecordsInDatabase.GetOrDefault(permissionGroupRecord.Name);
        if (permissionGroupRecordInDatabase == null)
        {
          /* New group */
          _logger.LogDebug("Adding new permission group ({group})", permissionGroupRecord.Name);
          newRecords.Add(permissionGroupRecord);
          continue;
        }

        if (permissionGroupRecord.HasSameData(permissionGroupRecordInDatabase))
        {
          /* Not changed */
          _logger.LogDebug("Permission group ({group}) has same data", permissionGroupRecord.Name);
          continue;
        }

        /* Changed */
        _logger.LogDebug("Patching pemrission group ({group})", permissionGroupRecord.Name);
        permissionGroupRecordInDatabase.Patch(permissionGroupRecord);
        changedRecords.Add(permissionGroupRecordInDatabase);
      }

      /* Deleted */
      var deletedRecords = PermissionOptions.DeletedPermissionGroups.Any()
          ? permissionGroupRecordsInDatabase.Values
              .Where(x => PermissionOptions.DeletedPermissionGroups.Contains(x.Name))
              .ToArray()
          : Array.Empty<PermissionGroupDefinitionRecord>();

      if (newRecords.Any())
      {
        await PermissionGroupRepository.InsertManyAsync(newRecords, cancellationToken: cancellationToken);
      }

      if (changedRecords.Any())
      {
        await PermissionGroupRepository.UpdateManyAsync(changedRecords, cancellationToken: cancellationToken);
      }

      if (deletedRecords.Any())
      {
        await PermissionGroupRepository.DeleteManyAsync(deletedRecords, cancellationToken: cancellationToken);
      }

      return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
    }

    private async Task<bool> UpdateChangedPermissionsAsync(
        IEnumerable<PermissionDefinitionRecord> permissionRecords,
        CancellationToken cancellationToken = default)
    {
      var newRecords = new List<PermissionDefinitionRecord>();
      var changedRecords = new List<PermissionDefinitionRecord>();

      var permissionRecordsInDatabase = (await PermissionRepository.GetListAsync(cancellationToken: cancellationToken))
          .Where(x => x.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty) == string.Empty)
          .ToDictionary(x => x.Name);

      foreach (var permissionRecord in permissionRecords)
      {
        var permissionRecordInDatabase = permissionRecordsInDatabase.GetOrDefault(permissionRecord.Name);
        if (permissionRecordInDatabase == null)
        {
          /* New permission */
          _logger.LogDebug("Adding new permission ({permission})", permissionRecord.Name);
          newRecords.Add(permissionRecord);
          continue;
        }

        if (permissionRecord.HasSameData(permissionRecordInDatabase))
        {
          /* Not changed */
          _logger.LogDebug("Permission has same data ({permission})", permissionRecord.Name);
          continue;
        }

        /* Changed */
        _logger.LogDebug("Patching permission ({permission})", permissionRecord.Name);
        permissionRecordInDatabase.Patch(permissionRecord);
        changedRecords.Add(permissionRecordInDatabase);
      }

      /* Deleted */
      var deletedRecords = new List<PermissionDefinitionRecord>();

      if (PermissionOptions.DeletedPermissions.Any())
      {
        deletedRecords.AddRange(
            permissionRecordsInDatabase.Values
                .Where(x => PermissionOptions.DeletedPermissions.Contains(x.Name))
        );
      }

      if (PermissionOptions.DeletedPermissionGroups.Any())
      {
        deletedRecords.AddIfNotContains(
            permissionRecordsInDatabase.Values
                .Where(x => PermissionOptions.DeletedPermissionGroups.Contains(x.GroupName))
        );
      }

      if (newRecords.Any())
      {
        await PermissionRepository.InsertManyAsync(newRecords, cancellationToken: cancellationToken);
      }

      if (changedRecords.Any())
      {
        await PermissionRepository.UpdateManyAsync(changedRecords, cancellationToken: cancellationToken);
      }

      if (deletedRecords.Any())
      {
        await PermissionRepository.DeleteManyAsync(deletedRecords, cancellationToken: cancellationToken);
      }

      return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
    }

    private string GetApplicationDistributedLockKey()
    {
      return $"{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpPermissionUpdateLock";
    }

    private string GetCommonDistributedLockKey()
    {
      return $"{CacheOptions.KeyPrefix}_Common_AbpPermissionUpdateLock";
    }

    private string GetApplicationHashCacheKey()
    {
      return $"{currentTenant.Id}_{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpPermissionsHash";
    }

    private string GetCommonStampCacheKey()
    {
      return $"{CacheOptions.KeyPrefix}_AbpInMemoryPermissionCacheStamp";
    }

    private static string CalculateHash(
        PermissionGroupDefinitionRecord[] permissionGroupRecords,
        PermissionDefinitionRecord[] permissionRecords,
        IEnumerable<string> deletedPermissionGroups,
        IEnumerable<string> deletedPermissions)
    {
      var jsonSerializerOptions = new JsonSerializerOptions
      {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
          Modifiers =
                {
                    new AbpIgnorePropertiesModifiers<PermissionGroupDefinitionRecord, Guid>().CreateModifyAction(x => x.Id),
                    new AbpIgnorePropertiesModifiers<PermissionDefinitionRecord, Guid>().CreateModifyAction(x => x.Id)
                }
        }
      };

      var stringBuilder = new StringBuilder();

      stringBuilder.Append("PermissionGroupRecords:");
      stringBuilder.AppendLine(JsonSerializer.Serialize(permissionGroupRecords, jsonSerializerOptions));

      stringBuilder.Append("PermissionRecords:");
      stringBuilder.AppendLine(JsonSerializer.Serialize(permissionRecords, jsonSerializerOptions));

      stringBuilder.Append("DeletedPermissionGroups:");
      stringBuilder.AppendLine(deletedPermissionGroups.JoinAsString(","));

      stringBuilder.Append("DeletedPermission:");
      stringBuilder.Append(deletedPermissions.JoinAsString(","));

      return stringBuilder
          .ToString()
          .ToMd5();
    }
  }
}
