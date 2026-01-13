using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.TenantSettings;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp;
using VPortal.TenantManagement.Module;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/TenantSettings/")]
  public class TenantSettingsController : TenantManagementController, ITenantSettingsAppService
  {
    private readonly IVportalLogger<TenantSettingsController> logger;
    private readonly ITenantSettingsAppService appService;

    public TenantSettingsController(
        IVportalLogger<TenantSettingsController> logger,
        ITenantSettingsAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetTenantSettings")]
    public async Task<TenantSettingDto> GetTenantSettings(Guid? tenantId)
    {
      var result = await appService.GetTenantSettings(tenantId);
      return result;
    }

    [HttpPost("SetExternalProviderSettings")]
    public async Task<bool> SetExternalProviderSettings(SetTenantProviderSettingsRequestDto request)
    {
      var result = await appService.SetExternalProviderSettings(request);
      return result;
    }

    [HttpGet("GetTenantSystemHealthSettings")]
    public async Task<TenantSystemHealthSettingsDto> GetTenantSystemHealthSettingsAsync()
    {
      var result = await appService.GetTenantSystemHealthSettingsAsync();
      return result;
    }

    [HttpPost("UpdateTenantSystemHealthSettings")]
    public async Task<bool> UpdateTenantSystemHealthSettingsAsync(TenantSystemHealthSettingsDto request)
    {
      var result = await appService.UpdateTenantSystemHealthSettingsAsync(request);
      return result;
    }
  }
}
