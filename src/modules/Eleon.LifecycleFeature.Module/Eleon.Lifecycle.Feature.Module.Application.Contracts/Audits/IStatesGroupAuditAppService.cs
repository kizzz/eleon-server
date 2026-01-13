using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.Audits
{
  public interface IStatesGroupAuditAppService : IApplicationService
  {
    Task<bool> Add(StatesGroupAuditDto statesGroupAudit);
    Task<StatesGroupAuditDto> GetById(Guid id);
    Task<bool> Remove(Guid id);
    Task<bool> DeepCancel(string docType, string documentId);
    Task<PagedResultDto<StatesGroupAuditReportDto>> GetReports(PendingApprovalRequestDto input);
  }
}
