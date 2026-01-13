using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module.Repositories
{
  public class UserSettingRepository : EfCoreRepository<TenantManagementDbContext, UserSettingEntity, Guid>, IUserSettingRepository
  {
    private readonly IVportalLogger<UserSettingRepository> logger;

    public UserSettingRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<UserSettingRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<UserSettingEntity> GetUserSettingByUserId(Guid userId)
    {
      UserSettingEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        var list = await dbContext.UserSettings.Where(x => x.UserId == userId).ToListAsync();
        if (list != null && list.Count > 0)
        {
          result = list.First();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<UserSettingEntity>> GetUserSettingsByUserId(Guid userId)
    {
      List<UserSettingEntity> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.UserSettings.Where(x => x.UserId == userId).ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

  }
}
