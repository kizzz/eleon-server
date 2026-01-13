using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.EventServices
{
  public class CheckIfControlDelegatedEventService :
      IDistributedEventHandler<CheckControlDelegationMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CheckIfControlDelegatedEventService> logger;
    private readonly ControlDelegationDomainService controlDelegationDomainService;
    private readonly IResponseContext responseContext;

    public CheckIfControlDelegatedEventService(
        IVportalLogger<CheckIfControlDelegatedEventService> logger,
        ControlDelegationDomainService controlDelegationDomainService,
        IResponseContext responseContext)
    {
      this.logger = logger;
      this.controlDelegationDomainService = controlDelegationDomainService;
      this.responseContext = responseContext;
    }

    public async Task HandleEventAsync(CheckControlDelegationMsg eventData)
    {
      var response = new ControlDelegationCheckedMsg();
      try
      {
        response.IsDelegated = await controlDelegationDomainService.IsControlDelegated(eventData.DelegatedByUserId, eventData.DelegatedToUserId);
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
