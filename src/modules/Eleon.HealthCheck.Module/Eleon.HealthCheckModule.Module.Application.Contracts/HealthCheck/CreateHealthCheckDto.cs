using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class CreateHealthCheckDto
{
  public string Type { get; set; }
  public string InitiatorName { get; set; }
}
