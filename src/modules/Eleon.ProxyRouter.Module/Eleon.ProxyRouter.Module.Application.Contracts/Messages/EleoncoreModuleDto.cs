using ProxyRouter.Minimal.HttpApi.Models.Constants;

namespace ProxyRouter.Minimal.HttpApi.Models.Messages
{
  public class EleoncoreModuleDto
  {
    public Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public required string Path { get; set; }
    public ModuleType Type { get; set; }
    public bool IsSystem { get; set; }
    public required string Source { get; set; }
    public bool IsHidden { get; set; }
    #region healthcheck
    public bool IsHealthCheckEnabled { get; set; }
    public DateTime? LastHealthCheckStatusDate { get; set; }
    public required string HealthCheckStatusMessage { get; set; }
    public ServiceHealthStatus HealthCheckStatus { get; set; }
    #endregion
  }
}
