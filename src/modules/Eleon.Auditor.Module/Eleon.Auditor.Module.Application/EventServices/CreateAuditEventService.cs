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
  public class CreateAuditEventService :
      IDistributedEventHandler<CreateAuditMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CreateAuditEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly AuditDomainService domainService;

    public CreateAuditEventService(
        IVportalLogger<CreateAuditEventService> logger,
        IResponseContext responseContext,
        AuditDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(CreateAuditMsg eventData)
    {
      var msg = eventData;
      var response = new AuditCreatedMsg();
      try
      {
        response.CreatedSuccessfully = await domainService.CreateAudit(
            msg.RefDocumentObjectType,
            msg.RefDocumentId,
            msg.AuditedDocumentObjectType,
            msg.AuditedDocumentId,
            msg.DocumentData,
            msg.DocumentVersion);
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
