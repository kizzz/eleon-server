using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.Audit;

[DistributedEvent]
public class AddAuditMsg : VportalEvent
{
  public AuditLogEto AuditLogInfo { get; set; }
}
