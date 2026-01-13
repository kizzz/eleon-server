using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;

namespace EleonsoftSdk.modules.HealthCheck.Module.Base;

public interface IEleonsoftHealthCheck
{
  string Name { get; }
  bool IsPublic { get; }
  Task<HealthCheckReportEto> CheckAsync(Guid healthCheckId);
}
