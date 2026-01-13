using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
public class AddHealthCheckReportDto
{
  public string ServiceName { get; set; }
  public string ServiceVersion { get; set; }
  public TimeSpan UpTime { get; set; }
  public string CheckName { get; set; }
  public HealthCheckStatus Status { get; set; }
  public string Message { get; set; }
  public bool IsPublic { get; set; }
  public Guid HealthCheckId { get; set; }
  public List<ReportExtraInformationDto> ExtraInformation { get; set; }
}
