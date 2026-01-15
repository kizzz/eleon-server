using Common.Module.Constants;
using Eleon.Notificator.Module.Eleon.Notificator.Module.Domain.Shared.Services;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using Logging.Module;
using Messaging.Module.ETO;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using Org.BouncyCastle.Cms;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.PushNotifications;
using VPortal.Notificator.Module.WebPush;

namespace VPortal.Notificator.Module.Notificators.Implementations
{
  public class PushNotificator : ITransientDependency
  {
    private readonly IVportalLogger<PushNotificator> logger;
    private readonly IPushNotificationHubContext pushNotificationHubContext;
    private readonly WebPushDomainService webPushDomainService;
    private readonly NotificatorHelperService _helperService;
    private readonly ISystemHubContext _systemHubContext;

    public PushNotificator(
            IVportalLogger<PushNotificator> logger,
            IPushNotificationHubContext pushNotificationHubContext,
            WebPushDomainService webPushDomainService,
            NotificatorHelperService helperService,
            ISystemHubContext systemHubContext)
    {
      this.logger = logger;
      this.pushNotificationHubContext = pushNotificationHubContext;
      this.webPushDomainService = webPushDomainService;
      this._helperService = helperService;
      _systemHubContext = systemHubContext;
    }

    public async Task SendPushAsync(List<Guid> userIds, string message, List<string> languageKeyParams, bool isLocalizedData, string redirectUrl, string applicationName, NotificationPriority priority, NotificationSourceType sourceType = NotificationSourceType.Notification, bool isNewMessage = true)
    {
      var notification = new PushNotificationValueObject
      {
        CreationTime = DateTime.UtcNow,
        Content = message,
        LanguageKeyParams = languageKeyParams,
        IsLocalizedData = isLocalizedData,
        IsRedirectEnabled = !string.IsNullOrWhiteSpace(redirectUrl),
        RedirectUrl = redirectUrl,
        ApplicationName = applicationName,
        Priority = priority,
        IsNewMessage = isNewMessage
      };
      await pushNotificationHubContext.PushNotification(userIds, notification);
      await webPushDomainService.PushMessage(userIds, message, sourceType);
    }

    public async Task SendSystemAsync(List<Guid> userIds, string message, List<string> languageKeyParams, bool isLocalizedData, NotificationPriority priority, NotificationSourceType sourceType = NotificationSourceType.Notification)
    {
      var notification = new PushNotificationValueObject
      {
        CreationTime = DateTime.UtcNow,
        Content = message,
        LanguageKeyParams = languageKeyParams,
        IsLocalizedData = isLocalizedData,
        IsRedirectEnabled = false,
        RedirectUrl = string.Empty,
        ApplicationName = null,
        Priority = priority
      };
      await _systemHubContext.PushAsync(userIds, notification);
    }
  }
}
