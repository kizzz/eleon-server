using System;
using Volo.Abp.Domain.Repositories;
using VPortal.Auditor.Module.Entities;

namespace VPortal.Auditor.Module.Repositories
{
  public interface IAuditDataRepository : IBasicRepository<AuditDataEntity, Guid>
  {

  }
}
