namespace ServicesOrchestrator.Services;

public enum ServiceCommand { Start = 1, Stop = 2 } // used as desired-state too

public sealed class OrchestratorManifest
{
  public List<ServiceConfig> Services { get; set; } = new();
}

public sealed class ServiceConfig
{
  public string Name { get; set; } = default!;
  public string Type { get; set; } = default!; // "database" | "connection" | "app" | "webapp"
  public string[] Dependencies { get; set; } = Array.Empty<string>();

  // For database/connection
  public string? ConnectionString { get; set; }

  // For app/webapp
  public string? ExecutablePath { get; set; }
  public string? WorkingDirectory { get; set; }
  public Dictionary<string, string>? Env { get; set; }
  public string? Arguments { get; set; }
  // For webapp
  public string? Url { get; set; }

  public int? AllowedDownMs { get; set; }
}


public sealed class OrchestratorOptions
{
  public const string SectionName = "Orchestrator";
  public bool Enabled { get; set; }
  public int DefaultTickMs { get; set; } = 5000;
  public int MinTickMs { get; set; } = 100;
  public int MaxTickMs { get; set; } = 5 * 60_000;
  public int AllowedDownMs { get; set; } = 3 * 60_000;
}
