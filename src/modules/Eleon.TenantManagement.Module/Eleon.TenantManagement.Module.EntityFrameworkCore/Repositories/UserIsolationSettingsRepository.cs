using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module.Repositories
{
  internal class UserIsolationSettingsRepository : EfCoreRepository<TenantManagementDbContext, UserIsolationSettingsEntity, Guid>, IUserIsolationSettingsRepository
  {
    private readonly IVportalLogger<UserIsolationSettingsRepository> logger;

    public UserIsolationSettingsRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<UserIsolationSettingsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<UserIsolationSettingsEntity> GetByUserIdAsync(Guid userId)
    {
      UserIsolationSettingsEntity result = null;
      try
      {
        var db = await GetDbContextAsync();
        result = await db.UserIsolationSettings
            .FirstOrDefaultAsync(x => x.UserId == userId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
