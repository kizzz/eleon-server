using Common.Module.Constants;
using Logging.Module;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.EntityFrameworkCore;

namespace VPortal.Auditor.Module.Repositories
{
  public class AuditCurrentVersionRepository :
      EfCoreRepository<AuditorDbContext, AuditCurrentVersionEntity, Guid>,
      IAuditCurrentVersionRepository
  {
    private readonly IVportalLogger<AuditCurrentVersionRepository> logger;
    public AuditCurrentVersionRepository(
        IDbContextProvider<AuditorDbContext> dbContextProvider,
        IVportalLogger<AuditCurrentVersionRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<AuditCurrentVersionEntity> GetCurrentVersion(string refDocumentType, string refDocumentId)
    {
      AuditCurrentVersionEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        var currentVersion = dbContext.AuditCurrentVersions.FirstOrDefault(x => x.RefDocId == refDocumentId && x.RefDocumentType == refDocumentType);
        result = currentVersion;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
