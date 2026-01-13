using Common.EventBus.Module;
using Commons.Module.Messages.Identity;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.EventServices
{
  public class GetValidApiKeyEventHandler :
      IDistributedEventHandler<GetValidApiKeyRequestMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetValidApiKeyEventHandler> logger;
    private readonly IResponseContext responseContext;
    private readonly IObjectMapper objectMapper;
    private readonly ApiKeyDomainService domainService;

    public GetValidApiKeyEventHandler(
        IVportalLogger<GetValidApiKeyEventHandler> logger,
        IResponseContext responseContext,
        IObjectMapper objectMapper,
        ApiKeyDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.objectMapper = objectMapper;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(GetValidApiKeyRequestMsg eventData)
    {
      var response = new ValidApiKeyReponseMsg();
      try
      {
        var key = await domainService.ValidateApiKey(eventData.ApiKey);
        response.ApiKey = objectMapper.Map<ApiKeyEntity, IdentityApiKeyEto>(key);
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
