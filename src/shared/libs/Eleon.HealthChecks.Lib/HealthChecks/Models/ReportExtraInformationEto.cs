using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class ReportExtraInformationEto
{
  public string Key { get; set; }
  public string Value { get; set; }
  public ReportInformationSeverity Severity { get; set; } = ReportInformationSeverity.Info;
  public string Type { get; set; } = "simple";
}
