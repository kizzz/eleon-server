using Common.EventBus.Module;
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
  public class GetApiKeysEventService :
      IDistributedEventHandler<GetIdentityApiKeysMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetApiKeysEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly IObjectMapper objectMapper;
    private readonly ApiKeyDomainService domainService;

    public GetApiKeysEventService(
        IVportalLogger<GetApiKeysEventService> logger,
        IResponseContext responseContext,
        IObjectMapper objectMapper,
        ApiKeyDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.objectMapper = objectMapper;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(GetIdentityApiKeysMsg eventData)
    {
      var response = new IdentityApiKeysGotMsg();
      try
      {
        var keys = await domainService.GetApiKeys(eventData.TypesFilter);
        response.Keys = objectMapper.Map<List<ApiKeyEntity>, List<IdentityApiKeyEto>>(keys);
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
