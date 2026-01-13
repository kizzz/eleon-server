using Common.Module.Events;
using Messaging.Module.Messages;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.SystemHealth;

[DistributedEvent]
public class SystemHealthSettingsUpdatedMsg : VportalEvent
{
  public OtelOptions Telemetry { get; set; }
}
