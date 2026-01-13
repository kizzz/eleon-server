using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Entities;
using VPortal.Notificator.Module.Repositories;

namespace VPortal.Notificator.Module.WebPush
{
  public class WebPushDomainService : DomainService
  {
    private readonly IVportalLogger<WebPushDomainService> logger;
    private readonly IWebPushSubscriptionRepository webPushSubscriptionRepository;
    private readonly UserNotificationSettingsDomainService userNotificationSettingsDomainService;
    private readonly WebPushClientManager webPushClientManager;

    public WebPushDomainService(
        IVportalLogger<WebPushDomainService> logger,
        IWebPushSubscriptionRepository webPushSubscriptionRepository,
        UserNotificationSettingsDomainService userNotificationSettingsDomainService,
        WebPushClientManager webPushClientManager)
    {
      this.logger = logger;
      this.webPushSubscriptionRepository = webPushSubscriptionRepository;
      this.userNotificationSettingsDomainService = userNotificationSettingsDomainService;
      this.webPushClientManager = webPushClientManager;
    }

    public async Task AddSubscription(Guid userId, string endpoint, string p256dh, string auth)
    {
      try
      {
        if (endpoint.IsNullOrEmpty() || p256dh.IsNullOrEmpty() || auth.IsNullOrEmpty())
        {
          throw new Exception("Some data is missing");
        }

        var subs = await webPushSubscriptionRepository.GetByUsers(userId.ToSingleItemList());
        bool alreadyExists = subs.Any(x => x.Endpoint == endpoint && x.Auth == auth);
        if (!alreadyExists)
        {
          var sub = new WebPushSubscriptionEntity(GuidGenerator.Create(), userId, endpoint, p256dh, auth);
          await webPushSubscriptionRepository.InsertAsync(sub);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task PushMessage(List<Guid> recepientUserIds, string message, NotificationSourceType sourceType)
    {
      try
      {
        List<Guid> userIds;
        if (sourceType != NotificationSourceType.Undefined)
        {
          var settings = await userNotificationSettingsDomainService.GetUserNotificationSettings(recepientUserIds, sourceType);
          userIds = recepientUserIds.Where(x => settings[x].SendNative).ToList();

          var subs = await webPushSubscriptionRepository.GetByUsers(recepientUserIds);
          if (subs.Count > 0)
          {
            logger.Log.LogDebug($"Pushing a WebPush message to {subs.Count} subscriptions.");
            foreach (var sub in subs)
            {
              await webPushClientManager.PushNotification(sub, message);
            }
          }
          else
          {
            logger.Log.LogDebug("Tried to push a WebPush message, but no subscriptions were found.");
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
