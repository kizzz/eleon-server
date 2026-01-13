using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.ControlDelegations
{
  public class ControlDelegationUserRemovedEventService :
      IDistributedEventHandler<UserRemovedMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<ControlDelegationUserRemovedEventService> logger;
    private readonly ControlDelegationDomainService controlDelegationDomainService;

    public ControlDelegationUserRemovedEventService(
        IVportalLogger<ControlDelegationUserRemovedEventService> logger,
        ControlDelegationDomainService controlDelegationDomainService)
    {
      this.logger = logger;
      this.controlDelegationDomainService = controlDelegationDomainService;
    }

    public async Task HandleEventAsync(UserRemovedMsg eventData)
    {
      try
      {
        await controlDelegationDomainService.RemoveControlDelegationsRelatedToUser(eventData.UserId);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
