namespace ServicesOrchestrator.Services.Abstractions;
public sealed class OrchestratorStatusDto
{
  public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
  public bool Enabled { get; init; }
  public List<ServiceStatusDto> Services { get; init; } = new();
}

public sealed class ServiceStatusDto
{
  public string Name { get; init; } = default!;
  public string Type { get; init; } = default!;
  public bool Up { get; init; }

  public DateTimeOffset? LastUpAt { get; init; }
  public long DownForMs { get; init; }
  public int StartAttempts { get; init; }
  public int ConsecutiveDownTicks { get; init; }
  public string? LastError { get; init; }
  public IReadOnlyList<string> RecentErrors { get; init; } = Array.Empty<string>();
  public string[] Dependencies { get; init; } = Array.Empty<string>();
  public int AllowedDownMs { get; init; }
  public string Desired { get; init; } = "Start"; // "Start" | "Stop"
}
