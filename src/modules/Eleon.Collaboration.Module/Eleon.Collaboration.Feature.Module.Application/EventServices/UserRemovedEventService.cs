using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Collaboration.Feature.Module.DomainServices;

namespace VPortal.Collaboration.Feature.Module.EventServices
{
  public class UserRemovedEventService :
      IDistributedEventHandler<UserRemovedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<UserRemovedEventService> logger;
    private readonly ChatMemberDomainService chatMemberDomainService;
    private readonly SupportTicketDomainService supportTicketDomainService;

    public UserRemovedEventService(
        IVportalLogger<UserRemovedEventService> logger,
        ChatMemberDomainService chatMemberDomainService,
        SupportTicketDomainService supportTicketDomainService)
    {
      this.logger = logger;
      this.chatMemberDomainService = chatMemberDomainService;
      this.supportTicketDomainService = supportTicketDomainService;
    }

    public async Task HandleEventAsync(UserRemovedMsg eventData)
    {
      try
      {
        await supportTicketDomainService.ForceRemoveSupportTickets(eventData.UserId);
        await chatMemberDomainService.ForceRemoveUserMemberships(eventData.UserId);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
