using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.PermissionManagement;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.Identity.Module.EventServices
{
  public class GetUserSignInSettingEventService :
      IDistributedEventHandler<GetUserSignInSettingMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetUserSignInSettingEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly UserSettingDomainService userSettingDomainService;

    public GetUserSignInSettingEventService(
        IVportalLogger<GetUserSignInSettingEventService> logger,
        IResponseContext responseContext,
        UserSettingDomainService userSettingDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.userSettingDomainService = userSettingDomainService;
    }

    public async Task HandleEventAsync(GetUserSignInSettingMsg eventData)
    {
      var response = new GetUserSignInSettingGotMsg();
      try
      {
        var reply = await userSettingDomainService.GetUserSettingByUserId(eventData.UserId);
        if (reply != null)
        {
          response.UserId = reply.UserId;
          response.TwoFaNotificationType = reply.TwoFaNotificationType;
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
