using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.ExternalLink.Module.DomainServices;

namespace VPortal.ExternalLink.Module.EventServices
{
  public class DeleteExternalLinkEventService :
      IDistributedEventHandler<DeleteExternalLinkMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<DeleteExternalLinkEventService> logger;
    private readonly ExternalLinkDomainService externalLinkDomainService;
    private readonly IObjectMapper mapper;

    public DeleteExternalLinkEventService(
        IVportalLogger<DeleteExternalLinkEventService> logger,
        ExternalLinkDomainService externalLinkDomainService,
        IObjectMapper mapper)
    {
      this.logger = logger;
      this.externalLinkDomainService = externalLinkDomainService;
      this.mapper = mapper;
    }

    public async Task HandleEventAsync(DeleteExternalLinkMsg eventData)
    {

      try
      {
        await externalLinkDomainService.Delete(eventData.LinkCode);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }

    }
  }
}
