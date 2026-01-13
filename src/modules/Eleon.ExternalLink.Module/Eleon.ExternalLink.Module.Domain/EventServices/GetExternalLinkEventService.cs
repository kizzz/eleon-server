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
  public class GetExternalLinkEventService :
      IDistributedEventHandler<GetExternalLinkMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetExternalLinkEventService> logger;
    private readonly ExternalLinkDomainService externalLinkDomainService;
    private readonly IResponseContext responseContext;
    private readonly IObjectMapper objectMapper;

    public GetExternalLinkEventService(
        IVportalLogger<GetExternalLinkEventService> logger,
        ExternalLinkDomainService externalLinkDomainService,
        IResponseContext responseContext,
        IObjectMapper objectMapper)
    {
      this.logger = logger;
      this.externalLinkDomainService = externalLinkDomainService;
      this.responseContext = responseContext;
      this.objectMapper = objectMapper;
    }

    public async Task HandleEventAsync(GetExternalLinkMsg eventData)
    {
      var message = new SendExternalLinkMsg();
      try
      {
        var result = await externalLinkDomainService.GetLinkAsync(eventData.LinkCode);

        message.ExternalLinkEto = objectMapper.Map<ExternalLinkEntity, ExternalLinkEto>(result);
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
