using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using VPortal.Notificator.Module.Notifications;

namespace VPortal.Notificator.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Notificator/Notifications")]
  public class NotificationsController : NotificatorModuleController, INotificationAppService
  {
    private readonly INotificationAppService appService;
    private readonly IVportalLogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationAppService NotificationsAppService,
        IVportalLogger<NotificationsController> logger)
    {
      this.appService = NotificationsAppService;
      _logger = logger;
    }

    [HttpPost("Send")]
    public async Task<bool> SendAsync(NotificationDto input)
    {

      try
      {
        return await appService.SendAsync(input);
      }
      finally
      {
      }
    }

    [HttpPost("SendBulk")]
    public async Task<bool> SendBulkAsync(List<NotificationDto> input)
    {

      try
      {
        return await appService.SendBulkAsync(input);
      }
      finally
      {
      }
    }
  }
}
