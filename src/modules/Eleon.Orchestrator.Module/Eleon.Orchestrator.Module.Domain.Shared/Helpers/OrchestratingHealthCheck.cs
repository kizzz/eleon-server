using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using ServicesOrchestrator.Services.Abstractions;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.HealthChecks;


[ExposeServices(typeof(IEleonsoftHealthCheck))]
public class OrchestratingHealthCheck : IEleonsoftHealthCheck, ITransientDependency
{
  private readonly IOrchestratingService _orchestratingService;

  public OrchestratingHealthCheck(IOrchestratingService orchestratingService)
  {
    _orchestratingService = orchestratingService;
  }

  public string Name => "Orchestrating";

  public bool IsPublic => true;

  public async Task<HealthCheckReportEto> CheckAsync(Guid healthCheckId)
  {
    var state = await _orchestratingService.GetStatusAsync(default);

    return new HealthCheckReportEto
    {
      HealthCheckId = healthCheckId,
      Status = state.Enabled && state.Services.Any(x => !x.Up && x.DownForMs > x.AllowedDownMs) ? HealthCheckStatus.Failed : HealthCheckStatus.OK,
      Message = $"Orchestrating service is {(state.Enabled ? "running" : "not running")}.",
      ExtraInformation = new List<ReportExtraInformationEto>
            {
                new ReportExtraInformationEto { Key = "Enabled", Value = state.Enabled.ToString() },
                new ReportExtraInformationEto { Key = "ServiceStatus", Value = string.Join(", ", state.Services.Select(x => $"{x.Name}: {(x.Up ? "Up" : "Down")}")) },
                new ReportExtraInformationEto { Key = "TotalServices", Value = state.Services.Count.ToString() },
                new ReportExtraInformationEto { Key = "TotalDownServices", Value = state.Services.Count(x => !x.Up).ToString() }
            },
    };
  }
}
