using Logging.Module;
using System;
using System.Threading.Tasks;

namespace VPortal.Notificator.Module.WebPush
{
  public class WebPushAppService : NotificatorModuleAppService, IWebPushAppService
  {
    private readonly IVportalLogger<WebPushAppService> logger;
    private readonly WebPushDomainService domainService;

    public WebPushAppService(
        IVportalLogger<WebPushAppService> logger,
        WebPushDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<bool> AddWebPushSubscription(WebPushSubscriptionDto subscription)
    {
      bool result = false;
      try
      {
        await domainService.AddSubscription(CurrentUser.Id.Value, subscription.Endpoint, subscription.P256Dh, subscription.Auth);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
