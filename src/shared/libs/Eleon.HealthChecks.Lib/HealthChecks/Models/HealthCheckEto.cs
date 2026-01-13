using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
public class HealthCheckEto
{
  public Guid Id { get; set; }
  public string Type { get; set; }
  public string InitiatorName { get; set; }
  public HealthCheckStatus Status { get; set; }
  public DateTime CreationTime { get; set; }
  public List<HealthCheckReportEto> Reports { get; set; } = new List<HealthCheckReportEto>();
}
