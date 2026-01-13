using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.NotificationLogs;

namespace VPortal.Notificator.Module.PushNotifications
{
  public interface IPushNotificationAppService : IApplicationService
  {
    Task<bool> AcknowledgeNotificationRead(Guid notificationLogId);
    Task<PagedResultDto<NotificationLogDto>> GetUnreadNotificationsAsync(string applicationName, int skip, int take);
    Task<bool> AcknowledgeNotificationsBulkRead(List<Guid> notificationLogIds);
    Task<List<NotificationLogDto>> GetRecentNotificationsAsync(RecentNotificationLogListRequestDto notificationRequest);
    Task<int> GetTotalUnreadNotificationsAsync(string applicationName);
  }
}
