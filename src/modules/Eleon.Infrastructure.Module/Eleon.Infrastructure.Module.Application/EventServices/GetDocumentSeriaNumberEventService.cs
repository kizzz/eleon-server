using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Shared.Modules.Core.Module.DomainServices;

namespace VPortal.Infrastructure.Module.EventServices
{
  public class GetDocumentSeriaNumberEventService :
      IDistributedEventHandler<GetDocumentSeriaNumberMsg>,
      ITransientDependency
  {
    private readonly SeriesDomainService seriesDomainService;
    private readonly IResponseContext responseContext;
    private readonly IVportalLogger<GetDocumentSeriaNumberEventService> logger;

    public GetDocumentSeriaNumberEventService(
        SeriesDomainService seriesDomainService,
        IResponseContext responseContext,
        IVportalLogger<GetDocumentSeriaNumberEventService> logger)
    {
      this.seriesDomainService = seriesDomainService;
      this.responseContext = responseContext;
      this.logger = logger;
    }

    public async Task HandleEventAsync(GetDocumentSeriaNumberMsg eventData)
    {
      var response = new DocumentSeriaNumberGotMsg();
      try
      {
        response.SeriaNumber = await seriesDomainService.GetNextSeriaNumber(
            eventData.ObjectType,
            eventData.Prefix,
            eventData.RefId);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
