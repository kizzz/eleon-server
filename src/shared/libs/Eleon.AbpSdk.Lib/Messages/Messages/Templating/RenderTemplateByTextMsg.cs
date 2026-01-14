using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templating.Module.Messages
{
  [DistributedEvent]
  public class RenderTemplateByTextMsg : VportalEvent
  {
    public Dictionary<string, string> Placeholders { get; set; } = new Dictionary<string, string>();
    public string TemplateType { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
  }
}
