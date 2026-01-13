using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace VPortal.Auditor.Module.Remote
{
  public class AbpAuditStoreRemoteAppService : ModuleAppService, IAbpAuditStoreRemoteAppService
  {
    private readonly IAuditingStore auditingStore;
    private readonly IVportalLogger<AbpAuditStoreRemoteAppService> logger;

    public AbpAuditStoreRemoteAppService(IAuditingStore auditingStore, IVportalLogger<AbpAuditStoreRemoteAppService> logger)
    {
      this.auditingStore = auditingStore;
      this.logger = logger;
    }

    [DisableAuditing]
    public async Task<bool> SaveAuditLogInfo(AuditLogInfo auditInfo)
    {
      bool success = false;
      try
      {
        await auditingStore.SaveAsync(auditInfo);
        success = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return success;
    }
  }
}
