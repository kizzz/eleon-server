using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class HealthCheckDto
{
  public Guid Id { get; set; }
  public string Type { get; set; }
  public string InitiatorName { get; set; }
  public HealthCheckStatus Status { get; set; }
  public DateTime CreationTime { get; set; }
}
