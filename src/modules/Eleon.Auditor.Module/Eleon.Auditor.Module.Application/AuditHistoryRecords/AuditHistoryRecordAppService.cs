using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Auditor.Module.DomainServices;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.AuditHistoryRecords
{
  public class AuditHistoryRecordAppService : ModuleAppService, IAuditHistoryRecordAppService
  {
    private readonly IVportalLogger<AuditHistoryRecordAppService> logger;
    private readonly AuditDomainService domainService;

    public AuditHistoryRecordAppService(
        IVportalLogger<AuditHistoryRecordAppService> logger,
        AuditDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<PagedResultDto<DocumentVersionEntity>> GetDocumentHistory(DocumentHistoryRequest request)
    {
      PagedResultDto<DocumentVersionEntity> response = null;
      try
      {
        var (count, history) = await domainService.GetAuditDocumentHistory(
            request.DocumentObjectType,
            request.DocumentId,
            request.Sorting,
            request.MaxResultCount,
            request.SkipCount,
            request.FromDateFilter,
            request.ToDateFilter);
        response = new PagedResultDto<DocumentVersionEntity>(count, history);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }
  }
}
