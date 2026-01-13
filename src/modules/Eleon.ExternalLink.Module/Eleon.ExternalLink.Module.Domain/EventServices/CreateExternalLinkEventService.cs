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
  public class CreateExternalLinkEventService :
      IDistributedEventHandler<CreateExternalLinkMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CreateExternalLinkEventService> logger;
    private readonly ExternalLinkDomainService externalLinkDomainService;
    private readonly IResponseContext responseContext;
    private readonly IObjectMapper mapper;

    public CreateExternalLinkEventService(
        IVportalLogger<CreateExternalLinkEventService> logger,
        ExternalLinkDomainService externalLinkDomainService,
        IResponseContext responseContext,
        IObjectMapper mapper)
    {
      this.logger = logger;
      this.externalLinkDomainService = externalLinkDomainService;
      this.responseContext = responseContext;
      this.mapper = mapper;
    }

    public async Task HandleEventAsync(CreateExternalLinkMsg eventData)
    {
      var message = new SendExternalLinkCreatedMsg();
      try
      {
        var entityToCreate = mapper.Map<ExternalLinkEto, ExternalLinkEntity>(eventData.NewExternalLinkEto);
        var result = await externalLinkDomainService.Create(entityToCreate);

        message.ExternalLinkCreated = mapper.Map<ExternalLinkEntity, ExternalLinkEto>(result);
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
