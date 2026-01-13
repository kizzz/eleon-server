using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Otel.Module;
public interface ITelemetrySettingsProvider
{
  Task InitializeAsync();
}
