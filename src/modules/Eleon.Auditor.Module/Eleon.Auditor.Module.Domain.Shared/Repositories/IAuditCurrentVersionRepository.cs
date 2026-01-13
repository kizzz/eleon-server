using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Auditor.Module.Entities;

namespace VPortal.Auditor.Module.Repositories
{
  public interface IAuditCurrentVersionRepository : IBasicRepository<AuditCurrentVersionEntity, Guid>
  {
    Task<AuditCurrentVersionEntity> GetCurrentVersion(string refDocumentType, string refDocumentId);
  }
}
