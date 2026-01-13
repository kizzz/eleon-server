using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class ReportExtraInformationDto
{
  public string Key { get; set; }
  public string Value { get; set; }
  public ReportInformationSeverity Severity { get; set; }
  public string Type { get; set; }
}
