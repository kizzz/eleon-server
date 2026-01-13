using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Helpers.Module;
public static class DebugHelper
{
  public static void AttachDebuggerIfNeeded(IConfiguration configuration)
  {
    if (configuration.GetValue<bool>("EnableDebug") && !Debugger.IsAttached) // debugger helper
    {
      Debugger.Launch();
    }
  }
}
