using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Templating
{
  [DistributedEvent]
  public class BulkRenderNotificationTemplateMsg : VportalEvent
  {
    public List<Dictionary<string, string>> TemplatePlaceholders { get; set; } = new List<Dictionary<string, string>>();
    public string TemplateKey { get; set; }
  }
}
