using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
public class HealthCheckReportEto
{
  public string ServiceName { get; set; }
  public string ServiceVersion { get; set; }
  public TimeSpan UpTime { get; set; }
  public string CheckName { get; set; }
  public HealthCheckStatus Status { get; set; }
  public string Message { get; set; }
  public Guid? TenantId { get; set; }
  public bool IsPublic { get; set; }
  public Guid HealthCheckId { get; set; }
  public List<ReportExtraInformationEto> ExtraInformation { get; set; } = new();
}
