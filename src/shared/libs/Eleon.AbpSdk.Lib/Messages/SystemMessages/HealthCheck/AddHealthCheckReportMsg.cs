using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;

public class AddHealthCheckReportMsg
{
  public HealthCheckReportEto HealthCheckReport { get; set; }
}


public class AddHealthCheckReportBulkMsg
{
  public List<HealthCheckReportEto> HealthCheckReports { get; set; }
}
