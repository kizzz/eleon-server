using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.EntityFrameworkCore;

namespace VPortal.Notificator.Module.Repositories
{

  public class UserNotificationSettingsRepository : EfCoreRepository<NotificatorDbContext, UserNotificationSettingsEntity, Guid>, IUserNotificationSettingsRepository
  {
    private readonly IDbContextProvider<NotificatorDbContext> dbContextProvider;
    private readonly IVportalLogger<UserNotificationSettingsRepository> logger;
    public UserNotificationSettingsRepository(IDbContextProvider<NotificatorDbContext> dbContextProvider, IVportalLogger<UserNotificationSettingsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<UserNotificationSettingsEntity> GetSettings(Guid userId, NotificationSourceType sourceType)
    {
      UserNotificationSettingsEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.SourceType == sourceType);
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

    public async Task<List<UserNotificationSettingsEntity>> GetSettings(List<Guid> userIds, NotificationSourceType sourceType)
    {
      List<UserNotificationSettingsEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(x => x.SourceType == sourceType && userIds.Contains(x.UserId))
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
