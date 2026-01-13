using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Lifecycle.Feature.Module.DomainServices;

namespace VPortal.Lifecycle.Feature.Module.EventServices
{
  public class GetDocumentIdsByFilterEventService : IDistributedEventHandler<GetDocumentIdsByFilterMsg>, ITransientDependency
  {
    private readonly IVportalLogger<GetDocumentIdsByFilterEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly StatesGroupAuditDomainService statesGroupAuditDomainService;

    public GetDocumentIdsByFilterEventService(
        IVportalLogger<GetDocumentIdsByFilterEventService> logger,
        IResponseContext responseContext,
        StatesGroupAuditDomainService statesGroupAuditDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.statesGroupAuditDomainService = statesGroupAuditDomainService;
    }

    public async Task HandleEventAsync(GetDocumentIdsByFilterMsg eventData)
    {
      var response = new GetDocumentIdsByFilterGotMsg();
      try
      {
        response.Ids = await statesGroupAuditDomainService.GetDocumentIdsByFilter(
            eventData.DocumentObjectType,
            eventData.UserId,
            eventData.Roles,
            eventData.LifecycleStatuses);
        response.IsSuccess = true;
      }
      catch (Exception ex)
      {
        response.IsSuccess = false;
        response.ErrorMsg = ex.Message;
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }
    }
  }
}
