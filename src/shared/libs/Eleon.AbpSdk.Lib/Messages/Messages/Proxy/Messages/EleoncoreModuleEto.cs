using Common.Module.Constants;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.ETO
{
  public class EleoncoreModuleEto
  {
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public bool IsDefault { get; set; }
    public bool IsEnabled { get; set; }
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
