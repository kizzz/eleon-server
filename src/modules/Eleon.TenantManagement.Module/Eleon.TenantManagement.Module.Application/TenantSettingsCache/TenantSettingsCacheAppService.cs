using Core.Infrastructure.Module.TenantSettings;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.Domain.Services;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Permissions;
using VPortal.TenantManagement.Module.TenantSettings;
using VPortal.TenantManagement.Module.TenantSettingsCache;

namespace VPortal.TenantManagement.Module
{
  public class TenantSettingsCacheAppService : TenantManagementAppService, ITenantSettingsCacheAppService
  {
    private readonly IVportalLogger<TenantSettingsCacheAppService> logger;
    private readonly TenantSettingsCacheService tenantSettingsCache;

    public TenantSettingsCacheAppService(
        IVportalLogger<TenantSettingsCacheAppService> logger,
        TenantSettingsCacheService tenantSettingsCache)
    {
      this.logger = logger;
      this.tenantSettingsCache = tenantSettingsCache;
    }
    public async Task<IReadOnlyList<string>> GetApplicationUrls()
    {
      return await tenantSettingsCache.GetApplicationUrls();
    }

    public async Task<IReadOnlyList<Guid?>> GetInactiveTenants()
    {
      return await tenantSettingsCache.GetInactiveTenants();
    }

    public async Task<TenantFoundDto> GetTenantByUrl(string url)
    {
      var (isFound, tenantId) = await tenantSettingsCache.GetTenantIdByUrl(url);
      return new TenantFoundDto()
      {
        IsFound = isFound,
        TenantId = tenantId,
      };

    }
    public async Task<TenantSettingsCacheValueDto> GetTenantSettings(Guid tenantId)
    {
      TenantSettingsCacheValueDto result = null;
      try
      {
        var entity = await tenantSettingsCache.GetTenantSettings(tenantId == Guid.Empty ? null : tenantId);
        result = ObjectMapper.Map<TenantSettingsCacheValue, TenantSettingsCacheValueDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
