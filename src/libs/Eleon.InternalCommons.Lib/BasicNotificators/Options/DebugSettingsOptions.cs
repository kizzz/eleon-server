using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
using EleonsoftSdk.modules.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Options;
public class DebugSettingsOptions
{
  public List<string> DevEmails { get; set; } = new List<string>();
  public TelegramDebugSettingsOptions TelegramTelemetry { get; set; } = new TelegramDebugSettingsOptions();
  public EmailDebugSettingsOptions EmailTelemetry { get; set; } = new EmailDebugSettingsOptions();
}

public class TelegramDebugSettingsOptions : TelegramOptions
{
}

public class EmailDebugSettingsOptions
{
  public string EmailServerType { get; set; } = "Smtp"; // or AzureEws
  public VPortal.Notificator.Module.Emailing.SmtpEmailSettings Smtp { get; set; }
  public AzureEwsOptions AzureEws { get; set; }
}
