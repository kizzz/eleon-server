using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Notificator.Module.WebPush;

namespace VPortal.Notificator.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Notificator/WebPush")]
  public class WebPushController : NotificatorModuleController, IWebPushAppService
  {
    private readonly IWebPushAppService appService;
    private readonly IVportalLogger<WebPushController> logger;

    public WebPushController(
        IWebPushAppService appService,
        IVportalLogger<WebPushController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("AddWebPushSubscription")]
    public async Task<bool> AddWebPushSubscription(WebPushSubscriptionDto subscription)
    {

      var response = await appService.AddWebPushSubscription(subscription);

      return response;
    }
  }
}
