using Common.Module.Constants;
using Common.Module.Extensions;
using Common.Module.Permissions;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Validation.StringValues;

namespace VPortal.TenantManagement.Module.Permissions
{
  public class FeaturePermissionManager : ITransientDependency
  {
    private readonly IVportalLogger<FeaturePermissionManager> logger;
    private readonly CurrentTenant currentTenant;
    private readonly StringValueTypeSerializer stringValueTypeSerializer;
    private readonly IFeatureDefinitionRecordRepository featureDefinitionRecordRepository;
    private readonly IFeatureGroupDefinitionRecordRepository featureGroupDefinitionRecordRepository;
    private readonly IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository;
    private readonly IPermissionGroupDefinitionRecordRepository permissionGroupDefinitionRecordRepository;
    private readonly IConfiguration configuration;
    private readonly IGuidGenerator guidGenerator;
    private readonly IFeatureChecker featureChecker;
    private static readonly SemaphoreSlim featureUpdateSemaphore = new SemaphoreSlim(1, 1);

    public FeaturePermissionManager(
        IVportalLogger<FeaturePermissionManager> logger,
        CurrentTenant currentTenant,
        StringValueTypeSerializer stringValueTypeSerializer,
        IFeatureDefinitionRecordRepository featureDefinitionRecordRepository,
        IFeatureGroupDefinitionRecordRepository featureGroupDefinitionRecordRepository,
        IPermissionDefinitionRecordRepository permissionDefinitionRecordRepository,
        IPermissionGroupDefinitionRecordRepository permissionGroupDefinitionRecordRepository,
        IConfiguration configuration,
        IGuidGenerator guidGenerator,
        IFeatureChecker featureChecker)
    {
      this.logger = logger;
      this.currentTenant = currentTenant;
      this.stringValueTypeSerializer = stringValueTypeSerializer;
      this.featureDefinitionRecordRepository = featureDefinitionRecordRepository;
      this.featureGroupDefinitionRecordRepository = featureGroupDefinitionRecordRepository;
      this.permissionDefinitionRecordRepository = permissionDefinitionRecordRepository;
      this.permissionGroupDefinitionRecordRepository = permissionGroupDefinitionRecordRepository;
      this.configuration = configuration;
      this.guidGenerator = guidGenerator;
      this.featureChecker = featureChecker;
    }

    public async Task EnsureServiceFeatures(Guid serviceId, List<FeatureGroupDescription> features)
    {

      await featureUpdateSemaphore.WaitAsync();
      try
      {
        await UpdateFeatureGroups(serviceId, features);
        await UpdateFeatures(serviceId, features);
        await UpdatePermissionGroups(serviceId, features);
        await UpdatePermissions(serviceId, features);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
        featureUpdateSemaphore.Release();
      }

    }

    public async Task<List<string>> FilterPermissionsByFeatures(List<string> grantedPermissions)
    {

      try
      {
        var filtered = new List<string>(grantedPermissions);

        bool isHost = currentTenant.Id == null;
        if (bool.TryParse(configuration["TenantManagement:EnableMultiTenancy"], out bool isMultiTenant) && !isMultiTenant)
        {
          filtered.RemoveAll(permission => permission.StartsWith("AbpTenantManagement."));
        }

        if (bool.TryParse(configuration["ElsaWorkflows:Enable"], out bool enableElsa) && !enableElsa)
        {
          filtered.RemoveAll(permission => permission.StartsWith("CoreInfrastructure.Module.ElsaWorkflows"));
        }

        if (isHost)
        {
          return filtered;
        }

        var featurePacks = FeaturePackHelper.GetFeaturePacks();
        foreach (var permission in grantedPermissions)
        {
          var featurePack = GetFeaturePack(featurePacks, permission);
          bool isNonFeature = featurePack == FeaturePack.None;
          bool isAllowed =
              isNonFeature
              || await featureChecker.IsEnabledAsync($"{featurePack}.Enable");

          if (!isAllowed)
          {
            filtered.Remove(permission);
          }
        }

        return filtered;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return default;
    }

    private async Task UpdateFeatureGroups(Guid serviceId, List<FeatureGroupDescription> newGroups)
    {
      var allExisting = await featureGroupDefinitionRecordRepository.GetListAsync();
      var existingServiceGroups = allExisting.Where(group => group.GetProperty<Guid?>(PermissionConstants.SourceIdPropertyName) == serviceId).ToList();

      var dif = existingServiceGroups.Difference(
          newGroups.Select(x =>
          {
            var record = new FeatureGroupDefinitionRecord(guidGenerator.Create(), x.Name, x.DisplayName);
            record.SetProperty(PermissionConstants.SourceIdPropertyName, serviceId);
            return record;
          }).ToList(),
          x => x.Name);

      foreach (var (orig, changed) in dif.Updated)
      {
        orig.DisplayName = changed.DisplayName;
        changed.MapExtraPropertiesTo(orig);
      }

      var updatedGroups = dif.Updated.Select(x => x.Original);

      await featureGroupDefinitionRecordRepository.DeleteManyAsync(dif.Removed);
      await featureGroupDefinitionRecordRepository.InsertManyAsync(dif.Added);
      await featureGroupDefinitionRecordRepository.UpdateManyAsync(updatedGroups);
    }

    private async Task UpdateFeatures(Guid serviceId, List<FeatureGroupDescription> newGroups)
    {
      var allExisting = await featureDefinitionRecordRepository.GetListAsync();
      var existingServiceFeatures = allExisting.Where(group => group.GetProperty<Guid?>(PermissionConstants.SourceIdPropertyName) == serviceId).ToList();

      var dif = existingServiceFeatures.Difference(
          newGroups
          .SelectMany(x => x.Children.Select(f => (x.Name, f)))
          .Select(x =>
          {
            var record = new FeatureDefinitionRecord(
                      guidGenerator.Create(),
                      x.Name,
                      x.f.Name,
                      null,
                      displayName: x.f.DisplayName,
                      valueType: SerializeFeatureValueType(x.f.Type));
            record.AllowedProviders = string.Join(
                      ',',
                      TenantFeatureValueProvider.ProviderName,
                      EditionFeatureValueProvider.ProviderName,
                      DefaultValueFeatureValueProvider.ProviderName);
            record.SetProperty(PermissionConstants.SourceIdPropertyName, serviceId);
            return record;
          }).ToList(),
          x => x.Name);

      foreach (var (orig, changed) in dif.Updated)
      {
        orig.DisplayName = changed.DisplayName;
        orig.GroupName = changed.GroupName;
        changed.MapExtraPropertiesTo(orig);
      }

      var updatedFeatures = dif.Updated.Select(x => x.Original);
      await featureDefinitionRecordRepository.DeleteManyAsync(dif.Removed);
      await featureDefinitionRecordRepository.InsertManyAsync(dif.Added);
      await featureDefinitionRecordRepository.UpdateManyAsync(updatedFeatures);
    }

    private async Task UpdatePermissionGroups(Guid serviceId, List<FeatureGroupDescription> newGroups)
    {
      var allExisting = await permissionGroupDefinitionRecordRepository.GetListAsync();
      var existingServiceGroups = allExisting
          .Where(group => group.GetProperty<Guid?>(PermissionConstants.SourceIdPropertyName) == serviceId)
          .ToList();

      var dif = existingServiceGroups.Difference(
          newGroups
              .SelectMany(
                  fg => fg.Children.SelectMany(f => f.Permissions.Select(p => new
                  {
                    FeatureGroup = fg.Name,
                    Feature = f.Name,
                    PermissionGroup = p,
                  })))
              .Select(x =>
              {
                var record = new PermissionGroupDefinitionRecord(
                          guidGenerator.Create(),
                          x.PermissionGroup.Name,
                          x.PermissionGroup.DisplayName);
                record.SetProperty(PermissionConstants.SourceIdPropertyName, serviceId);
                record.SetProperty("FeatureName", x.Feature);
                return record;
              })
              .ToList(),
          x => x.Name);

      foreach (var (orig, changed) in dif.Updated)
      {
        orig.DisplayName = changed.DisplayName;
        changed.MapExtraPropertiesTo(orig);
      }

      var updatedFeatures = dif.Updated.Select(x => x.Original);
      await permissionGroupDefinitionRecordRepository.DeleteManyAsync(dif.Removed);
      await permissionGroupDefinitionRecordRepository.InsertManyAsync(dif.Added);
      await permissionGroupDefinitionRecordRepository.UpdateManyAsync(updatedFeatures);
    }

    private async Task UpdatePermissions(Guid serviceId, List<FeatureGroupDescription> newGroups)
    {
      var allExisting = await permissionDefinitionRecordRepository.GetListAsync();
      var existingServiceGroups = allExisting
          .Where(group => group.GetProperty<Guid?>(PermissionConstants.SourceIdPropertyName) == serviceId)
          .ToList();

      var dif = existingServiceGroups.Difference(
          newGroups
              .SelectMany(
                  fg => fg.Children.SelectMany(f => f.Permissions.SelectMany(pg => pg.Children.Select(p => new
                  {
                    FeatureGroup = fg.Name,
                    Feature = f.Name,
                    PermissionGroup = pg.Name,
                    Permission = p,
                  }))))
              .Select(x =>
              {
                var record = new PermissionDefinitionRecord(
                          guidGenerator.Create(),
                          x.PermissionGroup,
                          x.Permission.Name,
                          null,
                          x.Permission.DisplayName,
                          multiTenancySide: x.Permission.MultiTenancySide);
                record.Providers = string.Join(
                          ',',
                          UserPermissionValueProvider.ProviderName,
                          RolePermissionValueProvider.ProviderName);
                record.SetProperty(PermissionConstants.SourceIdPropertyName, serviceId);
                record.SetProperty("FeatureName", x.Feature);
                return record;
              })
              .ToList(),
          x => x.Name);

      foreach (var (orig, changed) in dif.Updated)
      {
        orig.DisplayName = changed.DisplayName;
        changed.MapExtraPropertiesTo(orig);
      }

      var updatedFeatures = dif.Updated.Select(x => x.Original);
      await permissionDefinitionRecordRepository.DeleteManyAsync(dif.Removed);
      await permissionDefinitionRecordRepository.InsertManyAsync(dif.Added);
      await permissionDefinitionRecordRepository.UpdateManyAsync(updatedFeatures);
    }

    private string SerializeFeatureValueType(FeatureValueType featureValueType)
        => featureValueType switch
        {
          FeatureValueType.Text => stringValueTypeSerializer.Serialize(new FreeTextStringValueType()),
          FeatureValueType.Number => stringValueTypeSerializer.Serialize(new FreeTextStringValueType()),
          FeatureValueType.Toggle => stringValueTypeSerializer.Serialize(new ToggleStringValueType()),
          _ => throw new NotImplementedException(),
        };

    private static FeaturePack GetFeaturePack(List<FeaturePack> packs, string permission)
        => packs.FirstOrDefault(
            pack => permission.StartsWith($"Permission.{pack}."),
            FeaturePack.None);
  }
}
