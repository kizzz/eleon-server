using ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.Application.Contracts.Audit;
public interface IAuditAppService
{
  Task<bool> CreateAsync(CreateAuditDto input);
  Task<AuditDto> GetAsync(GetAuditDto input);
  Task<DocumentVersionEntity> GetCurrentVersionAsync(GetVersionDto input);
  Task<IncrementVersionResultDto> IncrementAuditVersionAsync(IncrementVersionRequestDto input);
}
