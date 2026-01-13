

using Common.Module.Extensions;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides;
using NUglify.Helpers;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;

namespace VPortal.TenantManagement.Module.DomainServices;

public class CustomFeaturesDomainService : DomainService
{
  private readonly IVportalLogger<CustomFeaturesDomainService> logger;
  private readonly IFeatureDefinitionRecordRepository featureDefinitionRecordRepository;
  private readonly CustomDynamicFeatureDefinitionStore dynamicFeatureDefinitionStore;
  private readonly IFeatureGroupDefinitionRecordRepository featureGroupDefinitionRecordRepository;

  public CustomFeaturesDomainService(
          IVportalLogger<CustomFeaturesDomainService> logger,
          IFeatureDefinitionRecordRepository featureDefinitionRecordRepository,
          CustomDynamicFeatureDefinitionStore dynamicFeatureDefinitionStore,
          IFeatureGroupDefinitionRecordRepository featureGroupDefinitionRecordRepository
      )
  {
    this.logger = logger;
    this.featureDefinitionRecordRepository = featureDefinitionRecordRepository;
    this.dynamicFeatureDefinitionStore = dynamicFeatureDefinitionStore;
    this.featureGroupDefinitionRecordRepository = featureGroupDefinitionRecordRepository;
  }

  public async Task<bool> CreateGroupsForMicroserviceAsync(string sourceId, List<FeatureGroupDefinitionRecord> groups)
  {
    bool result = false;
    try
    {
      var allExisting = await featureGroupDefinitionRecordRepository.GetListAsync();
      var existingServiceGroups = allExisting
          .Where(group => group.GetProperty<string>(PermissionConstants.SourceIdPropertyName) == sourceId)
          .ToList();

      var dif = existingServiceGroups.Difference(
          groups
              //.Where(g => !allExisting.Any(p => p.Name == g.Name && p.GetProperty<string>(PermissionConstants.SourceIdPropertyName) != sourceId))
              .Select(x =>
              {
                var record = new FeatureGroupDefinitionRecord(
                          GuidGenerator.Create(),
                          x.Name,
                          x.DisplayName);
                record.SetProperty(PermissionConstants.SourceIdPropertyName, sourceId);
                record.SetProperty("Dynamic", true);
                return record;
              })
              .ToList(),
          x => x.Name);

      foreach (var (orig, changed) in dif.Updated)
      {
        orig.DisplayName = changed.DisplayName;
        changed.ExtraProperties.ForEach(p => orig.SetProperty(p.Key, p.Value));
      }

      var updatedFeatures = dif.Updated.Select(x => x.Original);
      await featureGroupDefinitionRecordRepository.DeleteManyAsync(dif.Removed, true);
      await featureGroupDefinitionRecordRepository.InsertManyAsync(dif.Added, true);
      await featureGroupDefinitionRecordRepository.UpdateManyAsync(updatedFeatures, true);
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();

      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  public async Task<bool> CreateFeaturesForMicroserviceAsync(string sourceId, List<FeatureDefinitionRecord> features)
  {
    bool result = false;
    try
    {

      var allExisting = await featureDefinitionRecordRepository.GetListAsync();
      var existingServiceGroups = allExisting
          .Where(group => group.GetProperty<string>(PermissionConstants.SourceIdPropertyName) == sourceId)
          .ToList();

      var dif = existingServiceGroups.Difference(
          features
              //.Where(g => !allExisting.Any(p => p.Name == g.Name && p.GetProperty<string>(PermissionConstants.SourceIdPropertyName) != sourceId))
              .Select(x =>
              {
                var record = new FeatureDefinitionRecord(
                          GuidGenerator.Create(),
                          x.GroupName,
                          x.Name,
                          x.ParentName,
                          x.DisplayName,
                          x.Description,
                          x.DefaultValue,
                          x.IsVisibleToClients,
                          x.IsAvailableToHost,
                          allowedProviders: x.AllowedProviders,
                          valueType: x.ValueType
                          );
                record.AllowedProviders = string.Join(
                          ',',
                          UserPermissionValueProvider.ProviderName,
                          RolePermissionValueProvider.ProviderName);
                record.SetProperty(PermissionConstants.SourceIdPropertyName, sourceId);
                record.SetProperty("Dynamic", true);
                return record;
              })
              .ToList(),
          x => x.Name);

      foreach (var (orig, changed) in dif.Updated)
      {
        orig.DisplayName = changed.DisplayName;
        changed.ExtraProperties.ForEach(p => orig.SetProperty(p.Key, p.Value));
      }

      var updatedFeatures = dif.Updated.Select(x => x.Original);
      await featureDefinitionRecordRepository.DeleteManyAsync(dif.Removed, true);
      await featureDefinitionRecordRepository.InsertManyAsync(dif.Added, true);
      await featureDefinitionRecordRepository.UpdateManyAsync(updatedFeatures, true);
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  public async Task<FeatureGroupDefinitionRecord> CreateGroupAsync(FeatureGroupDefinitionRecord createEntity)
  {
    FeatureGroupDefinitionRecord result = null;
    try
    {
      createEntity.SetProperty("Dynamic", true);
      result = await featureGroupDefinitionRecordRepository.InsertAsync(createEntity, true);
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  public async Task<FeatureGroupDefinitionRecord> UpdateGroupAsync(FeatureGroupDefinitionRecord updateEntity)
  {
    FeatureGroupDefinitionRecord result = null;
    try
    {
      updateEntity.SetProperty("Dynamic", true);

      var existGroup = await (await featureGroupDefinitionRecordRepository.GetDbSetAsync()).AsNoTracking().FirstAsync(g => g.Id == updateEntity.Id);

      if (updateEntity.Name != existGroup.Name)
      {
        var updatedFeatures = await
            (await featureDefinitionRecordRepository.GetDbSetAsync())
            .Where(f => f.GroupName == existGroup.Name)
            .ToListAsync();

        if (updatedFeatures.Any())
        {
          foreach (var feature in updatedFeatures)
          {
            feature.GroupName = updateEntity.Name;
          }

          await featureDefinitionRecordRepository.UpdateManyAsync(updatedFeatures, true);
        }
      }

      result = await featureGroupDefinitionRecordRepository.UpdateAsync(updateEntity, true);

      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
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
      var dbSet = await featureGroupDefinitionRecordRepository.GetDbSetAsync();
      var entity = await dbSet.FirstAsync((e) => e.Name == name);

      var next = await
              (await featureDefinitionRecordRepository.GetDbSetAsync())
              .Where(f => f.GroupName == name)
              .ToListAsync();
      await RecursiveDeleteFeatures(next);

      await featureGroupDefinitionRecordRepository.DeleteAsync(entity, true);
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

  }

  public async Task<List<FeatureGroupDefinitionRecord>> GetFeatureDynamicGroupCategories()
  {
    List<FeatureGroupDefinitionRecord> result = null;
    try
    {
      result = await featureGroupDefinitionRecordRepository.GetListAsync();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  public async Task<FeatureDefinitionRecord> CreateAsync(FeatureDefinitionRecord createEntity)
  {
    FeatureDefinitionRecord result = null;
    try
    {
      createEntity.SetProperty("Dynamic", true);
      result = await featureDefinitionRecordRepository.InsertAsync(createEntity, true);
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  public async Task<FeatureDefinitionRecord> UpdateAsync(FeatureDefinitionRecord updateEntity)
  {
    FeatureDefinitionRecord result = null;
    try
    {
      var existFeature = await (await featureDefinitionRecordRepository.GetDbSetAsync()).AsNoTracking().FirstAsync(g => g.Id == updateEntity.Id);

      updateEntity.SetProperty("Dynamic", true);
      result = await featureDefinitionRecordRepository.UpdateAsync(updateEntity, true);
      if (updateEntity.Name != existFeature.Name)
      {
        var updatedFeatures = await
            (await featureDefinitionRecordRepository.GetDbSetAsync())
            .Where(f => f.ParentName == existFeature.Name)
            .ToListAsync();

        if (updatedFeatures.Any())
        {
          foreach (var feature in updatedFeatures)
          {
            feature.ParentName = updateEntity.Name;
          }

          await featureDefinitionRecordRepository.UpdateManyAsync(updatedFeatures, true);
        }
      }
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
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
      var entity = await featureDefinitionRecordRepository.FindByNameAsync(name);
      var next = await
              (await featureDefinitionRecordRepository.GetDbSetAsync())
              .Where(f => f.ParentName == name)
              .ToListAsync();
      await RecursiveDeleteFeatures(next);
      await featureDefinitionRecordRepository.DeleteAsync(entity, true);
      await dynamicFeatureDefinitionStore.UpdateInMemoryCache();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

  }

  public async Task<List<FeatureDefinitionRecord>> GetFeaturesDynamic()
  {
    List<FeatureDefinitionRecord> result = null;
    try
    {
      result = await featureDefinitionRecordRepository.GetListAsync();
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  private async Task RecursiveDeleteFeatures(IEnumerable<FeatureDefinitionRecord> features)
  {
    foreach (var feature in features)
    {

      var next = await
              (await featureDefinitionRecordRepository.GetDbSetAsync())
              .Where(f => f.ParentName == feature.Name)
              .ToListAsync();
      await RecursiveDeleteFeatures(next);
    }
    await featureDefinitionRecordRepository.DeleteManyAsync(features);
  }
}
