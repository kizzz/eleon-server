using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class EleoncoreModuleDto
  {
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public string Path { get; set; }
    public ModuleType Type { get; set; }
    public string Source { get; set; }
    public bool IsSystem { get; set; }
    public bool IsHidden { get; set; }
    #region healthcheck
    public bool IsHealthCheckEnabled { get; set; }
    public DateTime? LastHealthCheckStatusDate { get; set; }
    public string HealthCheckStatusMessage { get; set; }
    public ServiceHealthStatus HealthCheckStatus { get; set; }
    #endregion
  }
}


