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

  public class NotificationsRepository : EfCoreRepository<NotificatorDbContext, NotificationEntity, Guid>, INotificationsRepository
  {
    private readonly IDbContextProvider<NotificatorDbContext> dbContextProvider;
    private readonly IVportalLogger<NotificationsRepository> logger;
    public NotificationsRepository(IDbContextProvider<NotificatorDbContext> dbContextProvider, IVportalLogger<NotificationsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<List<NotificationEntity>> GetActiveListAsync()
    {

      List<NotificationEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(notification => notification.IsActive)
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
