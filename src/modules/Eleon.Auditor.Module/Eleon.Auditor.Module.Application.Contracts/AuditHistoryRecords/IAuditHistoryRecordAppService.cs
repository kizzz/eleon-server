using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.AuditHistoryRecords
{
  public interface IAuditHistoryRecordAppService : IApplicationService
  {
    public Task<PagedResultDto<DocumentVersionEntity>> GetDocumentHistory(DocumentHistoryRequest request);
  }
}
