using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Auditor.Module.DomainServices;

namespace VPortal.Auditor.Module.EventServices
{
  public class GetAuditDocumentEventService :
      IDistributedEventHandler<GetAuditDocumentMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<GetAuditDocumentEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly AuditDomainService domainService;

    public GetAuditDocumentEventService(
        IVportalLogger<GetAuditDocumentEventService> logger,
        IResponseContext responseContext,
        AuditDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(GetAuditDocumentMsg eventData)
    {
      var msg = eventData;
      var response = new AuditDocumentGotMsg();
      try
      {
        var versionHistoryRecord = await domainService.GetAuditDocument(msg.AuditedDocumentObjectType, msg.AuditedDocumentId, msg.Version);
        response.AuditedDocument = new AuditedDocumentEto
        {
          Data = versionHistoryRecord.data,
          Version = versionHistoryRecord.version,
        };
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
