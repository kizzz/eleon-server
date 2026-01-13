using Common.EventBus.Module;
using EleonsoftAbp.Messages.ApiKey;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.EventServices
{
  public class DeleteApiKeyEventService :
      IDistributedEventHandler<DeleteIdentityApiKeyMsg>,
      IDistributedEventHandler<DeleteApiKeyRequestMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<DeleteApiKeyEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly ApiKeyDomainService domainService;

    public DeleteApiKeyEventService(
        IVportalLogger<DeleteApiKeyEventService> logger,
        IResponseContext responseContext,
        ApiKeyDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(DeleteIdentityApiKeyMsg eventData)
    {
      var response = new ActionCompletedMsg();
      try
      {
        await domainService.RemoveApiKey(eventData.Type, eventData.Subject);
        response.Success = true;
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }

    public async Task HandleEventAsync(DeleteApiKeyRequestMsg eventData)
    {
      try
      {
        if (Guid.TryParse(eventData.ApiKeyId, out var apiKeyId))
        {
          await domainService.RemoveApiKeyAsync(apiKeyId);
        }
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }
      finally
      {
      }
    }
  }
}
