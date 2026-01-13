using Common.EventBus.Module;
using EleonsoftAbp.Messages.ApiKey;
using Logging.Module;
using Messaging.Module.Messages;
using SharedModule.modules.Helpers.Module.EventHandlers;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.DomainServices;

namespace VPortal.Identity.Module.EventServices
{
  public class UpdateApiKeyEventService :
      ConcurrencyAwareEventHandler<ApiKeyUpdatedMsg, UpdateApiKeyEventService>
  {
    private readonly IResponseContext responseContext;
    private readonly ApiKeyDomainService domainService;

    public UpdateApiKeyEventService(
        IVportalLogger<UpdateApiKeyEventService> logger,
        IResponseContext responseContext,
        ApiKeyDomainService domainService)
        : base(logger)
    {
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    protected override async Task HandleEventInternalAsync(ApiKeyUpdatedMsg eventData)
    {
      if (!Guid.TryParse(eventData.ApiKeyId, out var apiKeyId))
      {
        throw new Exception("Invalid Api key id");
      }

      var apiKey = await domainService.FindByIdAsync(apiKeyId);

      if (apiKey == null)
      {
        throw new Exception("Api key not found");
      }

      await domainService.UpdateAsync(apiKeyId, eventData.Name, apiKey.RefId, eventData.AllowAuthorize, apiKey.Data);
    }

  }
}
