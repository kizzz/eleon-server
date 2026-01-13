using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.EntityFrameworkCore;

namespace VPortal.Notificator.Module.Repositories
{

  public class NotificationLogRepository : EfCoreRepository<NotificatorDbContext, NotificationLogEntity, Guid>, INotificationLogRepository
  {
    private readonly IDbContextProvider<NotificatorDbContext> dbContextProvider;
    private readonly IVportalLogger<NotificationLogRepository> logger;
    public NotificationLogRepository(IDbContextProvider<NotificatorDbContext> dbContextProvider, IVportalLogger<NotificationLogRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<KeyValuePair<int, List<NotificationLogEntity>>> GetListAsync(
        Guid userId,
        int skip,
        int take,
        string applicationName,
        string sorting,
        List<NotificationType> typeFilter,
        DateTime? fromDate,
        DateTime? toDate)
    {
      KeyValuePair<int, List<NotificationLogEntity>> result = default;
      try
      {
        var dbSet = await GetDbSetAsync();
        var filtered = dbSet
            .Where(x => x.UserId == userId)
            .WhereIf(!applicationName.IsNullOrEmpty(), x => x.ApplicationName == applicationName)
            //.WhereIf(!typeFilter.IsNullOrEmpty(), x => typeFilter.Contains(x.Type))
            .WhereIf(fromDate != null, x => x.CreationTime >= fromDate)
            .WhereIf(toDate != null, x => x.CreationTime <= toDate);

        var count = await filtered.CountAsync();
        var paginated = await filtered
            .OrderBy(sorting)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        result = KeyValuePair.Create(count, paginated);
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

    public async Task<List<NotificationLogEntity>> GetLogsByIds(List<Guid> notificationLogIds)
    {
      List<NotificationLogEntity> result = new();
      try
      {
        var dbSet = await GetDbSetAsync();
        result = dbSet.Where(x => notificationLogIds.Contains(x.Id)).ToList();
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
