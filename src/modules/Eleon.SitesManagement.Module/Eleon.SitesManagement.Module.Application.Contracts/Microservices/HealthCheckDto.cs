using Common.Module.Constants;
using System;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class HealthCheckDto
  {
    public Guid Id { get; set; }
    public string HealthCheckEndpointUrl { get; set; }
    public DateTime? LastHealthCheck { get; set; }
    public ServiceHealthStatus Status { get; set; }
    public int HealthCheckPeriodMinutes { get; set; }
    public HealthCheckType HealthCheckType { get; set; }
    public bool IsEnabled { get; set; }
  }
}


