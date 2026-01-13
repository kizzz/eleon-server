using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Auditor.Module.DomainServices;

namespace VPortal.Auditor.Module.EventServices
{
  public class IncrementAuditVersionEventService :
      IDistributedEventHandler<IncrementAuditDocumentVersionMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<IncrementAuditVersionEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly AuditDomainService domainService;

    public IncrementAuditVersionEventService(
        IVportalLogger<IncrementAuditVersionEventService> logger,
        IResponseContext responseContext,
        AuditDomainService domainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(IncrementAuditDocumentVersionMsg eventData)
    {
      var msg = eventData;
      var response = new AuditVersionIncrementedMsg();
      try
      {
        var (success, newVersion) = await domainService.IncrementAuditVersion(msg.AuditedDocumentObjectType, msg.AuditedDocumentId, msg.Version);
        response.Success = success;
        response.NewVersion = newVersion;
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
