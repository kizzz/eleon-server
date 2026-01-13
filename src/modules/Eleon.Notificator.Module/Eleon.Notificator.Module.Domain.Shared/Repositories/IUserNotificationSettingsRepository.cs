using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.Repositories
{
  public interface IUserNotificationSettingsRepository : IBasicRepository<UserNotificationSettingsEntity>
  {
    Task<UserNotificationSettingsEntity> GetSettings(Guid userId, NotificationSourceType sourceType);
    Task<List<UserNotificationSettingsEntity>> GetSettings(List<Guid> userIds, NotificationSourceType sourceType);
  }
}
