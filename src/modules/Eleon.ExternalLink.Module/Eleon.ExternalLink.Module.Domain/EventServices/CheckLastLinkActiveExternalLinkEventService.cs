using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.ExternalLink.Module.DomainServices;
using VPortal.ExternalLink.Module.Entities;

namespace VPortal.ExternalLink.Module.EventServices
{
  public class CheckLastLinkActiveExternalLinkEventService :
      IDistributedEventHandler<CheckLastLinkActiveMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CheckLastLinkActiveExternalLinkEventService> logger;
    private readonly ExternalLinkDomainService externalLinkDomainService;
    private readonly IResponseContext responseContext;
    private readonly IObjectMapper mapper;

    public CheckLastLinkActiveExternalLinkEventService(
        IVportalLogger<CheckLastLinkActiveExternalLinkEventService> logger,
        ExternalLinkDomainService externalLinkDomainService,
        IResponseContext responseContext,
        IObjectMapper mapper)
    {
      this.logger = logger;
      this.externalLinkDomainService = externalLinkDomainService;
      this.responseContext = responseContext;
      this.mapper = mapper;
    }

    public async Task HandleEventAsync(CheckLastLinkActiveMsg eventData)
    {
      var message = new ActionCompletedMsg();
      try
      {
        message.Success = await externalLinkDomainService.CheckLastLinkActive(eventData.PrivateParams, eventData.ObjectType);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(message);
      }

    }
  }
}
