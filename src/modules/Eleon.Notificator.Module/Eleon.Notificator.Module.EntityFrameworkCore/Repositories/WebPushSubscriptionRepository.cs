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
  public class WebPushSubscriptionRepository : EfCoreRepository<NotificatorDbContext, WebPushSubscriptionEntity, Guid>, IWebPushSubscriptionRepository
  {
    private readonly IDbContextProvider<NotificatorDbContext> dbContextProvider;
    private readonly IVportalLogger<NotificationsRepository> logger;
    public WebPushSubscriptionRepository(IDbContextProvider<NotificatorDbContext> dbContextProvider, IVportalLogger<NotificationsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<List<WebPushSubscriptionEntity>> GetByUsers(List<Guid> userIds)
    {
      List<WebPushSubscriptionEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(x => userIds.Contains(x.UserId))
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
