using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using VPortal.Host;
using VPortal.TenantManagement.Module;

namespace VPortal.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/VPortal/Host")]
  public class HostController : AbpController, IHostAppService
  {
    private readonly IHostAppService HostAppService;
    private readonly IVportalLogger<HostController> _logger;

    public HostController(
        IHostAppService HostAppService,
        IVportalLogger<HostController> logger)
    {
      this.HostAppService = HostAppService;
      _logger = logger;
    }

    [HttpPost("Migrate")]
    public async Task MigrateAsync(Guid id)
    {

      await HostAppService.MigrateAsync(id);


      return;
    }
  }
}
