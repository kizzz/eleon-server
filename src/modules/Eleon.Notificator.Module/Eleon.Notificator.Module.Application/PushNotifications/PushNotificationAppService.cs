using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.NotificationLogs;
using VPortal.Notificator.Module.Notifications;

namespace VPortal.Notificator.Module.PushNotifications
{
  public class PushNotificationAppService : NotificatorModuleAppService, IPushNotificationAppService
  {
    private readonly IVportalLogger<NotificationAppService> logger;
    private readonly PushNotificationDomainService domainService;

    public PushNotificationAppService(
        IVportalLogger<NotificationAppService> logger,
        PushNotificationDomainService domainService
    )
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<bool> AcknowledgeNotificationRead(Guid notificationLogId)
    {
      bool result = false;
      try
      {
        await domainService.AcknowledgeNotificationRead(notificationLogId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }


    public async Task<bool> AcknowledgeNotificationsBulkRead(List<Guid> notificationLogIds)
    {
      bool result = false;
      try
      {
        await domainService.AcknowledgeNotificationsBulkRead(notificationLogIds);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<NotificationLogDto>> GetRecentNotificationsAsync(RecentNotificationLogListRequestDto notificationRequest)
    {
      List<NotificationLogDto> result = null;
      try
      {
        var entities = await domainService.GetRecentNotificationsAsync(notificationRequest.ApplicationName, notificationRequest.SkipCount, notificationRequest.MaxResultCount);
        result = ObjectMapper.Map<List<NotificationLogEntity>, List<NotificationLogDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      return result;
    }

    public async Task<int> GetTotalUnreadNotificationsAsync(string applicationName)
    {
      int result = 0;
      try
      {
        result = await domainService.GetTotalUnreadNotificationsAsync(applicationName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      return result;
    }

    public async Task<PagedResultDto<NotificationLogDto>> GetUnreadNotificationsAsync(string applicationName, int skip, int take)
    {
      PagedResultDto<NotificationLogDto> result = null;
      try
      {
        var list = await domainService.GetUnreadNotificationsAsync(applicationName, skip, take);
        var items = ObjectMapper.Map<List<NotificationLogEntity>, List<NotificationLogDto>>(list.Value);
        result = new(list.Key, items);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
