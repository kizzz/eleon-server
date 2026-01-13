namespace ServicesOrchestrator.Services.Abstractions;

public interface IOrchestratingConfigurationService
{
  OrchestratorOptions Options { get; }
  OrchestratorManifest Manifest { get; }
}
