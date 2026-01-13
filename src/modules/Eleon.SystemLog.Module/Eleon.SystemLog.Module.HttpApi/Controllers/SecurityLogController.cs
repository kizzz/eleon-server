using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.DocMessageLog.Module;
using VPortal.Infrastructure.Module;
using VPortal.Infrastructure.Module.SecurityLogs;

namespace VPortal.Infrastructure.Feature.Module.Controllers
{
  [Area(SystemLogModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SystemLogModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/SecurityLog/SecurityLogs")]
  public class SecurityLogController : SystemLogModuleController, ISecurityLogAppService
  {
    private readonly ISecurityLogAppService appService;
    private readonly IVportalLogger<SecurityLogController> _logger;

    public SecurityLogController(
        IVportalLogger<SecurityLogController> logger,
        ISecurityLogAppService appService)
    {
      _logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetSecurityLogById")]
    public Task<FullSecurityLogDto> GetSecurityLogByIdAsync(Guid id)
    {
      var response = appService.GetSecurityLogByIdAsync(id);
      return response;
    }

    [HttpPost("GetSecurityLogList")]
    public async Task<PagedResultDto<SecurityLogDto>> GetSecurityLogList(SecurityLogListRequestDto input)
    {

      var response = await appService.GetSecurityLogList(input);


      return response;
    }
  }
}
