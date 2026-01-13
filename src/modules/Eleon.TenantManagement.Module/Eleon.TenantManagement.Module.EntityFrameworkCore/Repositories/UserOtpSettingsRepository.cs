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
  internal class UserOtpSettingsRepository : EfCoreRepository<TenantManagementDbContext, UserOtpSettingsEntity, Guid>, IUserOtpSettingsRepository
  {
    private readonly IVportalLogger<UserOtpSettingsRepository> logger;

    public UserOtpSettingsRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<UserOtpSettingsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<UserOtpSettingsEntity> GetByUserIdAsync(Guid userId)
    {
      UserOtpSettingsEntity result = null;
      try
      {
        var db = await GetDbContextAsync();
        result = await db.UserOtpSettings
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
