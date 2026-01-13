using Logging.Module;
using Microsoft.Extensions.Logging;
using Migrations.Module;
using Sentry;
using System;
using System.Threading.Tasks;
using VPortal.Data;
using VPortal.TenantManagement.Module;

namespace VPortal.Host
{

  public class HostAppService : TenantManagementAppService, IHostAppService
  {
    private readonly IVportalLogger<HostAppService> logger;
    private readonly IDbMigrationService vPortalDbMigrationService;

    public HostAppService(
        IVportalLogger<HostAppService> logger,
        IDbMigrationService vPortalDbMigrationService
    )
    {
      this.logger = logger;
      this.vPortalDbMigrationService = vPortalDbMigrationService;
    }

    public async Task MigrateAsync(Guid id)
    {
      try
      {
        await vPortalDbMigrationService.MigrateTenantAsync(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

    }
  }

}
