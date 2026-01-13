using Logging.Module;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Core.Infrastructure.Module.Repositories;

namespace VPortal.Core.Infrastructure.Module
{

  public class FeatureSettingDomainService : DomainService
  {
    private readonly ICurrentTenant _currentTenant;
    private readonly IFeatureSettingsRepository _featureSettingsRepository;
    private readonly IDistributedCache<FeatureSettingEntity, FeatureSettingGroupInTenantCacheKey> _cache;
    private readonly IVportalLogger<FeatureSettingDomainService> _logger;

    public FeatureSettingDomainService(ICurrentTenant currentTenant, IFeatureSettingsRepository featureSettingsRepository,
        IDistributedCache<FeatureSettingEntity, FeatureSettingGroupInTenantCacheKey> cache, IVportalLogger<FeatureSettingDomainService> logger)
    {
      _currentTenant = currentTenant;
      _featureSettingsRepository = featureSettingsRepository;
      _logger = logger;
      _cache = cache;
    }

    public virtual async Task<List<FeatureSettingEntity>> SetAsync(Guid? tenantId, List<FeatureSettingEntity> settings)
    {
      //_logger.LogDebug(string.Format("FeatureSettingDomainService.SetAsync started with TenantId:{0}, Group:{1}, Key:{2}, Val:{3}, Type:{4}, IsEncrypted:{5}, IsRequired:{6}",
      //    tenantId.ToString(), group, key, val, type, isEncrypted, isRequired));

      List<FeatureSettingEntity> result = new();
      try
      {
        foreach (var setting in settings)
        {
          FeatureSettingEntity featureSetting = await this.GetFromDatabaseAsync(tenantId, setting.Group, setting.Key);
          if (featureSetting != null)
          {
            featureSetting.Value = setting.Value;
            featureSetting.Type = setting.Type;
            featureSetting.IsEncrypted = setting.IsEncrypted;
            featureSetting.IsRequired = setting.IsRequired;
            await _featureSettingsRepository.UpdateAsync(featureSetting);
          }
          else
          {
            featureSetting = await SetToDatabaseAsync(tenantId, setting.Group, setting.Key, setting.Value, setting.Type, setting.IsEncrypted, setting.IsRequired);
          }

          await _cache.SetAsync(
              new FeatureSettingGroupInTenantCacheKey(setting.Group, tenantId, setting.Key),
              setting,
              new DistributedCacheEntryOptions
              {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
              });
          result.Add(featureSetting);
        }
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public virtual async Task<FeatureSettingEntity> GetAsync(Guid? tenantId, string group, string key)
    {

      FeatureSettingEntity result = null;
      try
      {
        result = await _cache.GetOrAddAsync(
            new FeatureSettingGroupInTenantCacheKey(group, tenantId, key),
            async () => await GetFromDatabaseAsync(tenantId, group, key),
            () => new DistributedCacheEntryOptions
            {
              AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
            });
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<FeatureSettingEntity> GetFromDatabaseAsync(Guid? tenantId, string group, string key)
    {
      FeatureSettingEntity result = await _featureSettingsRepository.GetByKeyAsync(tenantId, group, key);

      return result;
    }

    private async Task<FeatureSettingEntity> SetToDatabaseAsync(
        Guid? tenantId, string group, string key, string val, string type, bool isEncrypted, bool isRequired)
    {
      FeatureSettingEntity setting = new(
          group,
          key,
          val,
          type,
          isEncrypted,
          isRequired,
          tenantId);

      return await _featureSettingsRepository.InsertAsync(setting, true);
    }
  }
}
