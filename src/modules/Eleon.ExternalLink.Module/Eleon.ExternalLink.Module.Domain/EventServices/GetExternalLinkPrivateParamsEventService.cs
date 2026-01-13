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
  public class GetExternalLinkPrivateParamsEventService :
      IDistributedEventHandler<GetExternalLinkPrivateParamsMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetExternalLinkPrivateParamsEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly ExternalLinkDomainService externalLinkDomainService;

    public GetExternalLinkPrivateParamsEventService(
        IVportalLogger<GetExternalLinkPrivateParamsEventService> logger,
        IResponseContext responseContext,
        ExternalLinkDomainService externalLinkDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.externalLinkDomainService = externalLinkDomainService;
    }

    public async Task HandleEventAsync(GetExternalLinkPrivateParamsMsg eventData)
    {
      var message = new SendExternalLinkPrivateParamsMsg();
      try
      {
        var result = await externalLinkDomainService.GetPrivateParams(eventData.LinkCode, eventData.Password);

        message.IsSuccess = result.IsSuccess;
        if (message.IsSuccess)
        {
          message.PrivateParams = result.Value;
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
