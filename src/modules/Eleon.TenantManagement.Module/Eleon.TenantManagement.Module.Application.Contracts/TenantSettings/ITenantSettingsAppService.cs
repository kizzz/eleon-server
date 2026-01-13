using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.TenantSettings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TenantSettings.Module.Cache
{
  public interface ITenantSettingsAppService : IApplicationService
  {
    Task<TenantSettingDto> GetTenantSettings(Guid? tenantId);
    Task<bool> SetExternalProviderSettings(SetTenantProviderSettingsRequestDto request);

    Task<TenantSystemHealthSettingsDto> GetTenantSystemHealthSettingsAsync();
    Task<bool> UpdateTenantSystemHealthSettingsAsync(TenantSystemHealthSettingsDto request);

  }
}
