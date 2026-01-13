using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators.Implementations;
public class TelegramOptions
{
  public bool Enabled { get; set; } = false;

  public string SystemBotToken { get; set; }
  public string SystemChatId { get; set; } = null!;

  public string BotToken { get; set; }
  public string ChatId { get; set; } = null!;
  public SystemLogLevel MinLogLevel { get; set; }
  public string MessageTemplate { get; set; }
  public string TemplateType { get; set; } = "Scriban";
}
