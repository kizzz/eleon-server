using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.ExternalLink.Module.DomainServices;

namespace VPortal.ExternalLink.Module.EventServices
{
  public class GetExternalLinkPublicParamsEventService :
      IDistributedEventHandler<GetExternalLinkPublicParamsMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetExternalLinkPublicParamsEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly ExternalLinkDomainService externalLinkDomainService;

    public GetExternalLinkPublicParamsEventService(
        IVportalLogger<GetExternalLinkPublicParamsEventService> logger,
        IResponseContext responseContext,
        ExternalLinkDomainService externalLinkDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.externalLinkDomainService = externalLinkDomainService;
    }

    public async Task HandleEventAsync(GetExternalLinkPublicParamsMsg eventData)
    {
      var message = new SendExternalLinkPublicParamsMsg();
      try
      {
        var result = await externalLinkDomainService.GetPublicParams(eventData.LinkCode);

        message.IsSuccess = result.IsSuccess;
        if (message.IsSuccess)
        {
          message.PublicParams = result.Value.PublicParams;
        }
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
