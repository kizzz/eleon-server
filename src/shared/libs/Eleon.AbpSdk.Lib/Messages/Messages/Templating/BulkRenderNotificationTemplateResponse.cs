using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Templating
{
  public class BulkRenderNotificationTemplateResponse
  {
    public List<string> RenderedTemplates { get; set; } = new List<string>();
  }
}
