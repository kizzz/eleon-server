using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.TenantManagement.Module.TenantSettings;
using VPortal.TenantManagement.Module.TenantSettingsCache;

namespace TenantSettings.Module.Cache
{
  public interface ITenantSettingsCacheAppService : IApplicationService
  {
    Task<IReadOnlyList<Guid?>> GetInactiveTenants();
    Task<IReadOnlyList<string>> GetApplicationUrls();
    Task<TenantSettingsCacheValueDto> GetTenantSettings(Guid tenantId);
    Task<TenantFoundDto> GetTenantByUrl(string url);
  }
}
