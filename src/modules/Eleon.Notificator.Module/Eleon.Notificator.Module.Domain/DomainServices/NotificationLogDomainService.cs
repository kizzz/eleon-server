using Common.Module.Constants;
using Eleon.Notificator.Module.Eleon.Notificator.Module.Domain.Shared.Options;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using Logging.Module;
using Microsoft.Extensions.Logging;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.Notificators.Implementations;
using VPortal.Notificator.Module.Repositories;
using VPortal.Notificator.Module.Settings;

namespace VPortal.Notificator.Module.DomainServices
{

  public class NotificationLogDomainService : DomainService
  {
    private readonly IVportalLogger<NotificationLogDomainService> logger;
    private readonly IdentityUserManager userManager;
    private readonly ICurrentUser currentUser;
    private readonly INotificationLogRepository notificationLogRepository;
    private readonly PushNotificator pushNotificator;
    private readonly ISettingManager _settingManager;

    public NotificationLogDomainService(
            IVportalLogger<NotificationLogDomainService> logger,
            IdentityUserManager userManager,
            ICurrentUser currentUser,
            INotificationLogRepository notificationLogRepository,
            PushNotificator pushNotificator,
            ISettingManager settingManager)
    {
      this.logger = logger;
      this.userManager = userManager;
      this.currentUser = currentUser;
      this.notificationLogRepository = notificationLogRepository;
      this.pushNotificator = pushNotificator;
      this._settingManager = settingManager;
    }

    public async Task<NotificationLogEntity> AddNotificationLogAsync(
            Guid? userId,
            string content,
            bool isLocalizedData,
            List<string> dataParams,
            string applicationName,
            bool isRedirectEnabled,
            string redirectUrl,
            NotificationPriority priority)
    {
      try
      {
        var log = new NotificationLogEntity(
            GuidGenerator.Create(),
            userId,
            content,
            isLocalizedData,
            string.Join(NotificatorConstants.DataSeparator, dataParams),
            applicationName,
            isRedirectEnabled,
            redirectUrl,
            priority
            );
        var result = await notificationLogRepository.InsertAsync(log, true);
        return result;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task<NotificationLogEntity> GetNotificationLogById(Guid notificationLogId)
    {
      NotificationLogEntity result = null;
      try
      {
        result = await notificationLogRepository.GetAsync(notificationLogId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<NotificationLogEntity>> GetNotificationLogByIds(List<Guid> notificationLogIds)
    {
      List<NotificationLogEntity> result = new();
      try
      {
        result = await notificationLogRepository.GetLogsByIds(notificationLogIds);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task AcknowledgeNotificationRead(Guid notificationId)
    {
      try
      {
        if (currentUser.Id == null)
        {
          throw new AbpAuthorizationException("Current user is not authenticated");
        }

        var log = await notificationLogRepository.GetAsync(notificationId);

        // Idempotency: check if already read
        if (log.IsRead)
        {
          logger.Log.LogInformation(
            "Notification {NotificationId} already marked as read. Treating as idempotent success.",
            notificationId);
          return;
        }

        log.IsRead = true;

        try
        {
          await notificationLogRepository.UpdateAsync(log, true);
        }
        catch (AbpDbConcurrencyException ex)
        {
          logger.Log.LogWarning(
            ex,
            "Concurrency conflict while acknowledging notification {NotificationId}. Waiting for desired state...",
            notificationId);

          await ConcurrencyExtensions.WaitForDesiredStateAsync(
            async () =>
            {
              var currentLog = await notificationLogRepository.GetAsync(notificationId);
              var isDesired = currentLog.IsRead;
              var details = $"IsRead={currentLog.IsRead}";
              return new ConcurrencyExtensions.ConcurrencyWaitResult<NotificationLogEntity>(isDesired, currentLog, details);
            },
            logger.Log,
            "AcknowledgeNotificationRead",
            notificationId
          );
        }
      }
      catch (AbpDbConcurrencyException)
      {
        throw; // Re-throw after handling above
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
    }

    public async Task AcknowledgeNotificationsBulkRead(List<Guid> notificationIds)
    {
      try
      {
        if (currentUser.Id == null)
        {
          throw new AbpAuthorizationException("Current user is not authenticated");
        }
        var logs = await notificationLogRepository.GetLogsByIds(notificationIds);

        // Idempotency: filter out already read notifications
        var unreadLogs = logs.Where(log => !log.IsRead).ToList();
        if (unreadLogs.Count == 0)
        {
          logger.Log.LogInformation(
            "All {Count} notifications already marked as read. Treating as idempotent success.",
            logs.Count);
          return;
        }

        foreach (var log in unreadLogs)
        {
          log.IsRead = true;
        }

        try
        {
          await notificationLogRepository.UpdateManyAsync(unreadLogs, true);
        }
        catch (AbpDbConcurrencyException ex)
        {
          logger.Log.LogWarning(
            ex,
            "Concurrency conflict while bulk acknowledging {Count} notifications. Waiting for desired state...",
            unreadLogs.Count);

          await ConcurrencyExtensions.WaitForDesiredStateAsync(
            async () =>
            {
              var currentLogs = await notificationLogRepository.GetLogsByIds(notificationIds);
              var stillUnread = currentLogs.Where(log => !log.IsRead).ToList();
              var isDesired = stillUnread.Count == 0;
              var details = $"UnreadCount={stillUnread.Count}";
              return new ConcurrencyExtensions.ConcurrencyWaitResult<List<NotificationLogEntity>>(isDesired, currentLogs, details);
            },
            logger.Log,
            "AcknowledgeNotificationsBulkRead",
            string.Join(",", notificationIds)
          );
        }
      }
      catch (AbpDbConcurrencyException)
      {
        throw; // Re-throw after handling above
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
    }

    public async Task<KeyValuePair<int, List<NotificationLogEntity>>> GetListAsync(int skip, int take, string applicationName, string sorting, List<NotificationType> typeFilter, DateTime? fromDate, DateTime? toDate, bool updateLastViewed = true)
    {
      KeyValuePair<int, List<NotificationLogEntity>> result = default;
      try
      {
        if (currentUser.Id == null)
        {
          throw new AbpAuthorizationException("Current user is not authenticated");
        }

        result = await notificationLogRepository.GetListAsync(currentUser.Id.Value, skip, take, applicationName, sorting, typeFilter, fromDate, toDate);

        string lastSorting = nameof(NotificationLogEntity.CreationTime) + " desc";
        var lastLog = (await notificationLogRepository.GetListAsync(currentUser.Id.Value, 0, 1, applicationName, lastSorting, typeFilter, null, null)).Value.FirstOrDefault();
        if (lastLog != null && updateLastViewed)
        {
          var state = await _settingManager.GetOrDefaultForCurrentUserAsync<PushNotificationUserStateOptions>(NotificatorModuleSettings.PushNotificationUserStateSettings);

          if (state.LastAckNotificationLog != lastLog.Id)
          {
            state.Acknowledge(lastLog);
            await _settingManager.SetForCurrentUserAsync(NotificatorModuleSettings.PushNotificationUserStateSettings, state);
            await this.pushNotificator.SendPushAsync(
                new List<Guid> { currentUser.Id.Value },
                string.Empty,
                new List<string>(),
                false,
                null,
                applicationName,
                NotificationPriority.Normal,
                NotificationSourceType.Notification,
                false);

          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;

    }
  }
}
