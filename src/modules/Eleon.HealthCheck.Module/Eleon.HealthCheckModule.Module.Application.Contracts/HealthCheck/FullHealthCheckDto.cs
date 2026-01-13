using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class FullHealthCheckDto : HealthCheckDto
{
  public Dictionary<string, object> ExtraProperties { get; set; } = new();
  public List<HealthCheckReportDto> Reports { get; set; }
}
