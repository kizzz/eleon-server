namespace VPortal.Notificator.Module.Settings;

public static class NotificatorModuleSettings
{
  public const string GroupName = "Notificator";

  /* Add constants for setting names. Example:
   * public const string MySettingName = GroupName + ".MySettingName";
   */

  public const string AzureEwsSettings = GroupName + ".AzureEws";
  public const string TelegramSettings = GroupName + ".Telegram";
  public const string GeneralSettings = GroupName + ".General";
  public const string PushNotificationUserStateSettings = GroupName + ".PushNotificationUserState";
}
