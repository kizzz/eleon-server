using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;

[DistributedEvent]
public class CreateHealthCheckResponseMsg : VportalEvent
{
  public bool Success { get; set; }
  public Guid HealthCheckId { get; set; }
  public string Error { get; set; }
}
