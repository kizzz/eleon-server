using Common.Module.Extensions;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Shared.Constants;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;

namespace VPortal.TenantManagement.Module.DomainServices
{
  public class CustomPermissionDomainService : DomainService
  {
    private readonly IVportalLogger<CustomPermissionDomainService> logger;
    private readonly IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository;
    private readonly CustomDynamicPermissionDefinitionStore dynamicPermissionDefinitionStore;
    private readonly IPermissionGroupDefinitionRecordRepository permissionGroupDefinitionRecordRepository;

    public CustomPermissionDomainService(
        IVportalLogger<CustomPermissionDomainService> logger,
        IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
        CustomDynamicPermissionDefinitionStore dynamicPermissionDefinitionStore,
        IPermissionGroupDefinitionRecordRepository permissionGroupDefinitionRecordRepository)
    {
      this.logger = logger;
      this.permissionDefinitionRecordRepository = permissionDefinitionRecordRepository;
      this.dynamicPermissionDefinitionStore = dynamicPermissionDefinitionStore;
      this.permissionGroupDefinitionRecordRepository = permissionGroupDefinitionRecordRepository;
    }
    public async Task<bool> CreateGroupsForMicroserviceAsync(string sourceId, List<PermissionGroupDefinitionRecord> groups)
    {
      bool result = false;
      try
      {
        var allExisting = await permissionGroupDefinitionRecordRepository.GetListAsync();
        var existingServiceGroups = allExisting
            .Where(group => group.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty) == sourceId)
            .ToList();

        var dif = existingServiceGroups.Difference(
            groups
                //.Where(g => !allExisting.Any(p => p.Name == g.Name && p.GetProperty<string>(PermissionConstants.SourceIdPropertyName) != sourceId))
                .Select(x =>
                {
                  var record = new PermissionGroupDefinitionRecord(
                              GuidGenerator.Create(),
                              x.Name,
                              x.DisplayName);
                  record.SetProperty(PermissionConstants.SourceIdPropertyName, sourceId);
                  record.SetProperty("CategoryName", x.ExtraProperties.GetOrDefault("CategoryName"));
                  record.SetProperty("Order", x.ExtraProperties.GetOrDefault("Order"));
                  record.SetProperty("Dynamic", true);
                  return record;
                })
                .ToList(),
            x => x.Name);

        // Store the changes to apply later
        var changesToApply = dif.Updated.ToList();

        await permissionGroupDefinitionRecordRepository.DeleteManyAsync(dif.Removed, true);
        await permissionGroupDefinitionRecordRepository.InsertManyAsync(dif.Added, true);

        // Reload entities before updating to avoid concurrency exceptions
        // This ensures we only update entities that still exist in the database
        var entitiesToUpdate = new List<PermissionGroupDefinitionRecord>();
        foreach (var (orig, changed) in changesToApply)
        {
          var reloadedEntity = await permissionGroupDefinitionRecordRepository.FindAsync(orig.Id);
          if (reloadedEntity != null)
          {
            reloadedEntity.DisplayName = changed.DisplayName;
            foreach (var p in changed.ExtraProperties)
            {
              reloadedEntity.SetProperty(p.Key, p.Value);
            }
            entitiesToUpdate.Add(reloadedEntity);
          }
        }

        if (entitiesToUpdate.Any())
        {
          await permissionGroupDefinitionRecordRepository.UpdateManyAsync(entitiesToUpdate, true);
        }
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task<bool> CreatePermissionsForMicroserviceAsync(string sourceId, List<PermissionDefinitionRecord> permissions)
    {
      bool result = false;
      try
      {

        var allExisting = await permissionDefinitionRecordRepository.GetListAsync();
        var existingServiceGroups = allExisting
            .Where(group => group.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty) == sourceId)
            .ToList();

        var dif = existingServiceGroups.Difference(
            permissions
                //.Where(g => !allExisting.Any(p => p.Name == g.Name && p.GetProperty<string>(PermissionConstants.SourceIdPropertyName) != sourceId))
                .Select(x =>
                {
                  var record = new PermissionDefinitionRecord(
                              GuidGenerator.Create(),
                              x.GroupName,
                              x.Name,
                              x.ParentName,
                              x.DisplayName,
                              multiTenancySide: x.MultiTenancySide);
                  record.Providers = string.Join(
                              ',',
                              UserPermissionValueProvider.ProviderName,
                              RolePermissionValueProvider.ProviderName,
                              ApiKeyConstants.ApiKeyPermissionProviderName);
                  record.SetProperty(PermissionConstants.SourceIdPropertyName, sourceId);
                  record.SetProperty("Order", int.TryParse(x.ExtraProperties.GetValueOrDefault("Order")?.ToString(), out var order) ? order : 0);
                  record.SetProperty("Dynamic", true);
                  return record;
                })
                .ToList(),
            x => x.Name);

        // Store the changes to apply later
        var changesToApply = dif.Updated.ToList();

        await permissionDefinitionRecordRepository.DeleteManyAsync(dif.Removed, true);
        await permissionDefinitionRecordRepository.InsertManyAsync(dif.Added.DistinctBy(x => x.Name), true);

        // Reload entities before updating to avoid concurrency exceptions
        // This ensures we only update entities that still exist in the database
        var entitiesToUpdate = new List<PermissionDefinitionRecord>();
        foreach (var (orig, changed) in changesToApply)
        {
          var reloadedEntity = await permissionDefinitionRecordRepository.FindAsync(orig.Id);
          if (reloadedEntity != null)
          {
            reloadedEntity.DisplayName = changed.DisplayName;
            reloadedEntity.ParentName = changed.ParentName;
            reloadedEntity.GroupName = changed.GroupName;
            reloadedEntity.IsEnabled = changed.IsEnabled;
            reloadedEntity.Providers = changed.Providers;
            reloadedEntity.StateCheckers = changed.StateCheckers;
            reloadedEntity.MultiTenancySide = changed.MultiTenancySide;

            foreach (var p in changed.ExtraProperties)
            {
              reloadedEntity.SetProperty(p.Key, p.Value);
            }
            entitiesToUpdate.Add(reloadedEntity);
          }
        }

        if (entitiesToUpdate.Any())
        {
          await permissionDefinitionRecordRepository.UpdateManyAsync(entitiesToUpdate, true);
        }
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task<PermissionGroupDefinitionRecord> CreateGroupAsync(PermissionGroupDefinitionRecord createEntity)
    {
      PermissionGroupDefinitionRecord result = null;
      try
      {
        createEntity.SetProperty("Dynamic", true);
        result = await permissionGroupDefinitionRecordRepository.InsertAsync(createEntity, true);
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task<PermissionGroupDefinitionRecord> UpdateGroupAsync(PermissionGroupDefinitionRecord updateEntity)
    {
      PermissionGroupDefinitionRecord result = null;
      try
      {
        updateEntity.SetProperty("Dynamic", true);
        result = await permissionGroupDefinitionRecordRepository.UpdateAsync(updateEntity, true);
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task DeleteGroupAsync(string name)
    {
      try
      {
        var dbSet = await permissionGroupDefinitionRecordRepository.GetDbSetAsync();
        var entity = await dbSet.FirstAsync((e) => e.Name == name);

        await permissionGroupDefinitionRecordRepository.DeleteAsync(entity, true);
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
    public async Task<List<PermissionGroupDefinitionRecord>> GetPermissionDynamicGroupCategories()
    {
      List<PermissionGroupDefinitionRecord> result = null;
      try
      {
        result = await permissionGroupDefinitionRecordRepository.GetListAsync();
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task<PermissionDefinitionRecord> CreateAsync(PermissionDefinitionRecord createEntity, int order)
    {
      PermissionDefinitionRecord result = null;
      try
      {
        createEntity.SetProperty("Dynamic", true);
        createEntity.SetProperty("Order", order);
        result = await permissionDefinitionRecordRepository.InsertAsync(createEntity, true);
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task<PermissionDefinitionRecord> UpdateAsync(PermissionDefinitionRecord updateEntity)
    {
      PermissionDefinitionRecord result = null;
      try
      {
        updateEntity.SetProperty("Dynamic", true);
        result = await permissionDefinitionRecordRepository.UpdateAsync(updateEntity, true);
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
    public async Task DeleteAsync(string name)
    {
      try
      {
        var dbSet = await permissionDefinitionRecordRepository.GetDbSetAsync();
        var entity = await dbSet.FirstAsync((e) => e.Name == name);

        await permissionDefinitionRecordRepository.DeleteAsync(entity, true);
        await dynamicPermissionDefinitionStore.UpdateInMemoryCache();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
    public async Task<List<PermissionDefinitionRecord>> GetPermissionsDynamic()
    {
      List<PermissionDefinitionRecord> result = null;
      try
      {
        result = await permissionDefinitionRecordRepository.GetListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

  }
}
