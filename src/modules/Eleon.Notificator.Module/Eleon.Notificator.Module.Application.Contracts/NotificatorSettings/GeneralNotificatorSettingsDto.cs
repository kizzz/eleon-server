using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Constants;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.NotificatorSettings;

public class GeneralNotificatorSettingsDto
{
  public string ServerType { get; set; } = "SMTP";
  public List<string> SystemEmails { get; set; } = new List<string>();
  public SystemLogLevel MinLogLevel { get; set; } = SystemLogLevel.Critical;
  public bool SendErrors { get; set; } = true;
  public string SystemEmailTemplate { get; set; } = NotificatorConstants.DefaultSystemMessageTemplate;
  public string TemplateType { get; set; } = "PlainText";
}
