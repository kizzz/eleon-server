using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.Repositories
{
  public interface INotificationLogRepository : IBasicRepository<NotificationLogEntity, Guid>
  {
    Task<KeyValuePair<int, List<NotificationLogEntity>>> GetListAsync(Guid userId, int skip, int take, string applicationName, string sorting, List<NotificationType> typeFilter, DateTime? fromDate, DateTime? toDate);
    Task<List<NotificationLogEntity>> GetLogsByIds(List<Guid> notificationLogIds);
  }
}
