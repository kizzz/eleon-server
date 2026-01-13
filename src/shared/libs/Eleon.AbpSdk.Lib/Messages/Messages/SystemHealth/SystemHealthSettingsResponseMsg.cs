using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.Messages.SystemHealth;
public class SystemHealthSettingsResponseMsg
{
  public OtelOptions Telemetry { get; set; }
  public Guid StorageProviderId { get; set; }
}
