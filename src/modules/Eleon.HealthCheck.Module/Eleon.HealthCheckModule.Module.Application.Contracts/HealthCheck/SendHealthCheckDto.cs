using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class SendHealthCheckDto
{
  public Guid Id { get; set; }
  public string Type { get; set; }
  public string InitiatorName { get; set; }
  public List<HealthCheckReportDto> Reports { get; set; }
}
