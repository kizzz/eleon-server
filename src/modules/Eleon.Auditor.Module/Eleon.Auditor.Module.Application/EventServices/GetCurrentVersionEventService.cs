using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Auditor.Module.DomainServices;

namespace VPortal.Auditor.Module.EventServices
{
  public class GetCurrentVersionEventService :
      IDistributedEventHandler<GetAuditCurrentVersionMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetCurrentVersionEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly AuditDomainService domainService;

    public GetCurrentVersionEventService(
        IVportalLogger<GetCurrentVersionEventService> logger,
        IResponseContext responseContext,
        AuditDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(GetAuditCurrentVersionMsg eventData)
    {
      var msg = eventData;
      var response = new AuditCurrentVersionGotMsg
      {
      };
      try
      {
        response.CurrentVersion = await domainService.GetCurrentVersion(msg.RefDocumentObjectType, msg.RefDocumentId);
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
