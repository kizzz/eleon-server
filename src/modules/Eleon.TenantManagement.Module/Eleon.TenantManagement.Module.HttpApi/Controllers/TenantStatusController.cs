using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.TenantStatuses;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/TenantStatus/")]
  public class TenantStatusController : TenantManagementController, ITenantStatusAppService
  {
    private readonly IVportalLogger<TenantStatusController> logger;
    private readonly ITenantStatusAppService appService;

    public TenantStatusController(
        IVportalLogger<TenantStatusController> logger,
        ITenantStatusAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("SuspendTenant")]
    public async Task<bool> SuspendTenant(Guid tenantId)
    {
      var result = await appService.SuspendTenant(tenantId);
      return result;
    }

    [HttpPost("UnsuspendTenant")]
    public async Task<bool> UnsuspendTenant(Guid tenantId)
    {
      var result = await appService.UnsuspendTenant(tenantId);
      return result;
    }
  }
}
