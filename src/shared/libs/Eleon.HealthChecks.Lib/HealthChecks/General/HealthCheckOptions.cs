using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Base;
public class HealthCheckOptions
{
  public bool Enabled { get; set; }
  public List<string> EnabledChecks { get; set; } = new List<string>();
  public string ApplicationName { get; set; } = "Undefined";
  public HealthCheckUIOptions UI { get; set; } = new HealthCheckUIOptions();
  public string HealthStatusPath { get; set; } = "/api/health";
  public int CheckTimeout { get; set; } = 5;
  public bool SendImmediately { get; set; } = true;
  public int SendIntervalMinutes { get; set; } = 5;
  public bool RestartEnabled { get; set; } = false;
  public bool RestartRequiresAuth { get; set; } = true;
  public bool EnableDiagnostics { get; set; } = false;
  public bool PublishOnFailure { get; set; } = true;
  public bool PublishOnChange { get; set; } = false;
  public int PublishIntervalMinutes { get; set; } = 5;
}

public class HealthCheckUIOptions
{
  public bool Enabled { get; set; }
  public string Path { get; set; } = "/healthchecks-ui";
  public bool ManualHealthCheckEnabled { get; set; } = true;
  public List<Navigate> Navigates { get; set; } = new List<Navigate>();
}

public class Navigate
{
  public string Name { get; set; } = "";
  public string Path { get; set; } = "/";
  public bool Enabled { get; set; } = true;
}
