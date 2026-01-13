using Core.Infrastructure.Module.Entities;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Infrastructure.Module.EntityFrameworkCore;

namespace Core.Infrastructure.Module.Repositories
{
  public class DashboardSettingRepository : EfCoreRepository<InfrastructureDbContext, DashboardSettingEntity, Guid>, IDashboardSettingRepository
  {
    private readonly IVportalLogger<DashboardSettingRepository> logger;

    public DashboardSettingRepository(
        IDbContextProvider<InfrastructureDbContext> dbContextProvider,
        IVportalLogger<DashboardSettingRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<DashboardSettingEntity>> GetDefaultDashboardSettings()
    {
      List<DashboardSettingEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(x => x.IsDefault)
            .ToListAsync();
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

    public async Task<List<DashboardSettingEntity>> GetList(Guid userId)
    {
      List<DashboardSettingEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(x => x.UserId == userId)
            .ToListAsync();
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
