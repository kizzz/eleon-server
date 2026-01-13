using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Notificator.Module.NotificationLogs;

namespace VPortal.Notificator.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Notificator/NotificationLogs")]
  public class NotificationLogController : NotificatorModuleController, INotificationLogAppService
  {
    private readonly INotificationLogAppService appService;
    private readonly IVportalLogger<NotificationLogController> logger;

    public NotificationLogController(
        INotificationLogAppService appService,
        IVportalLogger<NotificationLogController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpGet("GetNotificationLogList")]
    public async Task<PagedResultDto<NotificationLogDto>> GetNotificationLogList(NotificationLogListRequestDto request)
    {

      var response = await appService.GetNotificationLogList(request);

      return response;
    }
  }
}
