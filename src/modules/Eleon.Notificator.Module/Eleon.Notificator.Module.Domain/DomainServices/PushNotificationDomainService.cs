using Common.Module.Constants;
using Eleon.Notificator.Module.Eleon.Notificator.Module.Domain.Shared.Options;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using Logging.Module;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.Repositories;
using VPortal.Notificator.Module.Settings;

namespace VPortal.Notificator.Module.DomainServices
{

  public class PushNotificationDomainService : DomainService
  {
    private readonly IVportalLogger<PushNotificationDomainService> logger;
    private readonly CurrentUser currentUser;
    private readonly NotificationLogDomainService notificationLogDomainService;
    private readonly ISettingManager _settingManager;

    public PushNotificationDomainService(
        IVportalLogger<PushNotificationDomainService> logger,
        CurrentUser currentUser,
        NotificationLogDomainService notificationLogDomainService,
        ISettingManager settingManager)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.notificationLogDomainService = notificationLogDomainService;
      _settingManager = settingManager;
    }

    public async Task<KeyValuePair<int, List<NotificationLogEntity>>> GetUnreadNotificationsAsync(string applicationName, int skip, int take)
    {
      KeyValuePair<int, List<NotificationLogEntity>> result = default;
      try
      {
        var userId = (Guid)currentUser.Id;
        var state = await _settingManager.GetOrDefaultForCurrentUserAsync<PushNotificationUserStateOptions>(NotificatorModuleSettings.PushNotificationUserStateSettings);

        DateTime? lastAckDate = state?.LastAckDate == null ? null : state.LastAckDate.Value.AddSeconds(2);
        string sorting = nameof(NotificationLogEntity.CreationTime) + " desc";
        var typesFilter = new List<NotificationType> { NotificationType.Push };

        result = await notificationLogDomainService.GetListAsync(skip, take, applicationName, sorting, typesFilter, lastAckDate, null);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task AcknowledgeNotificationRead(Guid notificationLogId)
    {
      try
      {
        await notificationLogDomainService.AcknowledgeNotificationRead(notificationLogId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
    public async Task<List<NotificationLogEntity>> GetRecentNotificationsAsync(string applicationName, int skip, int take)
    {
      List<NotificationLogEntity> result = null;
      try
      {
        var userId = (Guid)currentUser.Id;
        var state = await _settingManager.GetOrDefaultForCurrentUserAsync<PushNotificationUserStateOptions>(NotificatorModuleSettings.PushNotificationUserStateSettings);

        string sorting = nameof(NotificationLogEntity.CreationTime) + " desc";
        var typesFilter = new List<NotificationType> { NotificationType.Push };
        var notificationList = await notificationLogDomainService.GetListAsync(skip, take, applicationName, sorting, typesFilter, null, null);

        result = notificationList.Value;
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
        var userId = (Guid)currentUser.Id;
        var state = await _settingManager.GetOrDefaultForCurrentUserAsync<PushNotificationUserStateOptions>(NotificatorModuleSettings.PushNotificationUserStateSettings);

        DateTime? lastAckDate = state?.LastAckDate == null ? null : state.LastAckDate.Value.AddSeconds(2);
        string sorting = nameof(NotificationLogEntity.CreationTime) + " desc";
        var typesFilter = new List<NotificationType> { NotificationType.Push };

        var notificationList = await notificationLogDomainService.GetListAsync(0, 100, applicationName, sorting, typesFilter, lastAckDate, null, false);
        result = notificationList.Key;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task AcknowledgeNotificationsBulkRead(List<Guid> notificationLogIds)
    {
      try
      {
        await notificationLogDomainService.AcknowledgeNotificationsBulkRead(notificationLogIds);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
