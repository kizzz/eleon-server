using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.ContentSecurityHosts;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/ContentSecurity/")]
  public class TenantContentSecurityController : TenantManagementController, ITenantContentSecurityAppService
  {
    private readonly IVportalLogger<TenantContentSecurityController> logger;
    private readonly ITenantContentSecurityAppService tenantContentSecurityAppService;

    public TenantContentSecurityController(
        IVportalLogger<TenantContentSecurityController> logger,
        ITenantContentSecurityAppService tenantContentSecurityAppService)
    {
      this.logger = logger;
      this.tenantContentSecurityAppService = tenantContentSecurityAppService;
    }

    [HttpPost("AddTenantContentSecurityHost")]
    public async Task<bool> AddTenantContentSecurityHost(AddTenantContentSecurityHostDto input)
    {

      var response = await tenantContentSecurityAppService.AddTenantContentSecurityHost(input);


      return response;
    }

    [HttpPost("RemoveTenantContentSecurityHost")]
    public async Task<bool> RemoveTenantContentSecurityHost(RemoveTenantContentSecurityHostDto input)
    {

      var response = await tenantContentSecurityAppService.RemoveTenantContentSecurityHost(input);


      return response;
    }

    [HttpPost("UpdateTenantContentSecurityHost")]
    public async Task<bool> UpdateTenantContentSecurityHost(UpdateTenantContentSecurityHostDto input)
    {

      var response = await tenantContentSecurityAppService.UpdateTenantContentSecurityHost(input);


      return response;
    }
  }
}
