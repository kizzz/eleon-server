using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class HealthCheckEntity : Entity<Guid>
  {
    public string HealthCheckEndpointUrl { get; set; }
    public DateTime? LastHealthCheck { get; set; }
    public ServiceHealthStatus Status { get; set; }
    public int HealthCheckPeriodMinutes { get; set; }
    public HealthCheckType HealthCheckType { get; set; }
    public bool IsEnabled { get; set; }
  }
}


