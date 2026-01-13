using Logging.Module;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.EntityFrameworkCore;

namespace VPortal.Auditor.Module.Repositories
{
  public class AuditDataRepository :
      EfCoreRepository<AuditorDbContext, AuditDataEntity, Guid>,
      IAuditDataRepository
  {
    private readonly IVportalLogger<AuditDataRepository> logger;
    public AuditDataRepository(
        IDbContextProvider<AuditorDbContext> dbContextProvider,
        IVportalLogger<AuditDataRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }
  }
}
