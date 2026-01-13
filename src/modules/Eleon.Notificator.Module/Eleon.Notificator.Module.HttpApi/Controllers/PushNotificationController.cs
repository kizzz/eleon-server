using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Notificator.Module.NotificationLogs;
using VPortal.Notificator.Module.PushNotifications;

namespace VPortal.Notificator.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Notificator/PushNotifications")]
  public class PushNotificationController : NotificatorModuleController, IPushNotificationAppService
  {
    private readonly IPushNotificationAppService appService;
    private readonly IVportalLogger<PushNotificationController> logger;

    public PushNotificationController(
        IPushNotificationAppService appService,
        IVportalLogger<PushNotificationController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("AcknowledgeNotificationRead")]
    public async Task<bool> AcknowledgeNotificationRead(Guid notificationLogId)
    {

      var response = await appService.AcknowledgeNotificationRead(notificationLogId);

      return response;
    }

    [HttpPost("AcknowledgeNotificationsBulkRead")]
    public async Task<bool> AcknowledgeNotificationsBulkRead(List<Guid> notificationLogIds)
    {

      var response = await appService.AcknowledgeNotificationsBulkRead(notificationLogIds);

      return response;
    }

    [HttpGet("GetUnreadNotifications")]
    public async Task<PagedResultDto<NotificationLogDto>> GetUnreadNotificationsAsync(string applicationName, int skip, int take)
    {

      var response = await appService.GetUnreadNotificationsAsync(applicationName, skip, take);

      return response;
    }

    [HttpGet("GetRecentNotifications")]
    public async Task<List<NotificationLogDto>> GetRecentNotificationsAsync(RecentNotificationLogListRequestDto notificationRequest)
    {

      var response = await appService.GetRecentNotificationsAsync(notificationRequest);

      return response;
    }

    [HttpGet("GetTotalUnreadNotifications")]
    public Task<int> GetTotalUnreadNotificationsAsync(string applicationName)
    {
      var response = appService.GetTotalUnreadNotificationsAsync(applicationName);
      return response;
    }
  }
}
