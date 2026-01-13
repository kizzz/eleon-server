using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.TenantSettings;
using VPortal.TenantManagement.Module.TenantSettingsCache;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/TenantSettingsCache/")]
  public class TenantSettingsCacheController : TenantManagementController, ITenantSettingsCacheAppService
  {
    private readonly IVportalLogger<TenantSettingsCacheController> logger;
    private readonly ITenantSettingsCacheAppService appService;

    public TenantSettingsCacheController(
        IVportalLogger<TenantSettingsCacheController> logger,
        ITenantSettingsCacheAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetApplicationUrls")]
    public async Task<IReadOnlyList<string>> GetApplicationUrls()
    {
      var result = await appService.GetApplicationUrls();
      return result;
    }

    [HttpGet("GetInactiveTenants")]
    public async Task<IReadOnlyList<Guid?>> GetInactiveTenants()
    {
      var result = await appService.GetInactiveTenants();
      return result;
    }
    [HttpGet("GetTenantByUrl")]
    public async Task<TenantFoundDto> GetTenantByUrl(string url)
    {
      var result = await appService.GetTenantByUrl(url);
      return result;
    }

    [HttpGet("GetTenantSettings")]
    public async Task<TenantSettingsCacheValueDto> GetTenantSettings(Guid tenantId)
    {
      var result = await appService.GetTenantSettings(tenantId);
      return result;
    }

  }
}
