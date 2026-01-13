using Microsoft.Toolkit.Uwp.Notifications;
using VPortal.ProxyClient.UI.Windows.Services;

namespace VPortal.ProxyClient.UI.Windows.WinUI;

public class NotificationService : INotificationService
{
    public void ShowNotification(string title, string body)
    {
        new ToastContentBuilder()
            .AddToastActivationInfo(null, ToastActivationType.Foreground)
            .AddAppLogoOverride(new Uri("ms-appx:///Assets/dotnet_bot.png"))
            .AddText(title, hintStyle: AdaptiveTextStyle.Header)
            .AddText(body, hintStyle: AdaptiveTextStyle.Body)
            .Show();
    }
}
