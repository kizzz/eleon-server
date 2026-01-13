using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Notificator.Module.DomainServices;

namespace VPortal.Notificator.Module.EventServices
{
  public class PushChatMessageEventService :
      IDistributedEventHandler<PushChatMessageMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<PushChatMessageEventService> logger;
    private readonly ChatPushDomainService domainService;

    public PushChatMessageEventService(
        IVportalLogger<PushChatMessageEventService> logger,
        ChatPushDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(PushChatMessageMsg eventData)
    {
      try
      {
        await domainService.PushMessage(eventData.Message, eventData.AudienceUserIds, eventData.AudienceRoles, eventData.AudienceOrgUnits);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
