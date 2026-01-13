using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.Constants;
public static class NotificatorConstants
{
  public const string DataSeparator = ";";
  public const string DefaultSystemMessageTemplate =
  "<b>💬 {logLevel}</b> <code>{priority}</code>{endline}" +
  "<b>🕒</b> {time}{endline}" +
  "<b>Message:</b> {message}{endline}" +
  "<b>Exception:</b> <code>{exception_message}</code>{endline}" +
  "<pre>{extraProperties}</pre>";

  public const string TelegramScribanTemplate = """
{{- $lvl = logLevel | string.downcase -}}
{{- $icon = "ℹ️" -}}
{{- if $lvl == "critical" -}}{{ $icon = "❌" -}}
{{- else if $lvl == "warning" -}}{{ $icon = "⚠️" -}}
{{- end -}}

<b>{{$icon}} {{ logLevel }}</b> <code>{{ priority }}</code>{{ endline }}
<b>🕒</b> {{ time }}{{ endline }}
<b>💬 Message:</b> {{ message }}{{ endline }}
<b>⚠️ Exception:</b> <code>{{ exception_message }}</code>{{ endline }}
<pre>{{ extraProperties }}</pre>
""";

  public class TemplateTypes
  {
    public const string Default = PlainText;
    public const string PlainText = "PlainText";
    public const string Scriban = "Scriban";
  }
}
