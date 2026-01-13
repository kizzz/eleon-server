using ServicesOrchestrator.Services.Abstractions;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services;

[ExposeServices(typeof(IOrchestratingConfigurationService))]
public sealed class AppsettingsOrchestratingConfigurationService : IOrchestratingConfigurationService, ISingletonDependency
{
  private readonly Microsoft.Extensions.Options.IOptionsMonitor<OrchestratorOptions> _opt;
  private readonly Microsoft.Extensions.Options.IOptionsMonitor<OrchestratorManifest> _manifest;

  public AppsettingsOrchestratingConfigurationService(
      Microsoft.Extensions.Options.IOptionsMonitor<OrchestratorOptions> opt,
      Microsoft.Extensions.Options.IOptionsMonitor<OrchestratorManifest> manifest)
  {
    _opt = opt;
    _manifest = manifest;
  }

  public OrchestratorOptions Options => _opt.CurrentValue;
  public OrchestratorManifest Manifest => _manifest.CurrentValue;
}
