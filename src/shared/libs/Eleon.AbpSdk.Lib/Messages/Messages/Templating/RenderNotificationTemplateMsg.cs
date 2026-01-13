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
  public class RenderNotificationTemplateMsg : VportalEvent
  {
    public Dictionary<string, string> Placeholders { get; set; } = new Dictionary<string, string>();
    public string TemplateName { get; set; } = string.Empty;
  }
}
