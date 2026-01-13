using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using Volo.Abp.Settings;

namespace VPortal.Notificator.Module.Settings;

public class NotificatorModuleSettingDefinitionProvider : SettingDefinitionProvider
{
  public override void Define(ISettingDefinitionContext context)
  {

    var fromDisplayName = context.GetOrNull("Abp.Mailing.DefaultFromDisplayName");
    fromDisplayName.DefaultValue = "Application";

    var fromAddress = context.GetOrNull("Abp.Mailing.DefaultFromAddress");
    fromAddress.DefaultValue = "noreply@";

    context.Add(new SettingDefinition(
        name: NotificatorModuleSettings.AzureEwsSettings,
        defaultValue: null,
        isVisibleToClients: true
        ));

    context.Add(new SettingDefinition(
        name: NotificatorModuleSettings.TelegramSettings,
        defaultValue: null,
        isVisibleToClients: true
        ));

    context.Add(new SettingDefinition(
        name: NotificatorModuleSettings.GeneralSettings,
        defaultValue: null,
        isVisibleToClients: true
        ));
    context.Add(new SettingDefinition(
    name: NotificatorModuleSettings.PushNotificationUserStateSettings,
    defaultValue: null,
    isVisibleToClients: true
    ));
  }
}
