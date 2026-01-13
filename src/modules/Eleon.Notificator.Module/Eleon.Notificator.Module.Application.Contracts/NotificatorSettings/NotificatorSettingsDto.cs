using EleonsoftModuleCollector.Commons.Module.Constants;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.NotificatorSettings;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.Emails;


namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
public class NotificatorSettingsDto
{
  public GeneralNotificatorSettingsDto GeneralSettings { get; set; }
  public SmtpSettingsDto SmtpSettings { get; set; }
  public AzureEwsSettingsDto AzureEws { get; set; }
  public TelegramSettingsDto Telegram { get; set; }
}
