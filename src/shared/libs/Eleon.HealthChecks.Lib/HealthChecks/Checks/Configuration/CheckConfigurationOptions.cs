using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
public class CheckConfigurationOptions
{
  public bool Enabled { get; set; } = false;
  public bool ThrowExceptionOnInvalid { get; set; } = false;
  public Dictionary<string, string> Configurations { get; set; } = new();
}
