using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.EventServices
{
  public class AddApiKeyEventService :
      IDistributedEventHandler<AddApiKeyMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<AddApiKeyEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly ApiKeyDomainService domainService;

    public AddApiKeyEventService(
        IVportalLogger<AddApiKeyEventService> logger,
        IResponseContext responseContext,
        ApiKeyDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(AddApiKeyMsg eventData)
    {
      var msg = eventData;
      var response = new ApiKeyAddedMsg
      {
        AddedSuccessfully = false
      };
      try
      {
        var key = await domainService.AddApiKeyAsync(msg.Name, msg.Subject, msg.Type, msg.AllowAuthorize, msg.Data, msg.ExpiresAt);
        response.AddedSuccessfully = key != null;
        response.ApiKeyId = key?.Id ?? Guid.Empty;
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
  }
}
