using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Constants;
using ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Emails;
public class TelegramSettingsDto
{
  public bool Enabled { get; set; }
  public string BotToken { get; set; }
  public string ChatId { get; set; }
  public string SystemBotToken { get; set; }
  public string SystemChatId { get; set; } = null!;
  public SystemLogLevel MinLogLevel { get; set; } = SystemLogLevel.Critical;
  public string MessageTemplate { get; set; } = NotificatorConstants.TelegramScribanTemplate;
  public string TemplateType { get; set; } = "Scriban";
}
