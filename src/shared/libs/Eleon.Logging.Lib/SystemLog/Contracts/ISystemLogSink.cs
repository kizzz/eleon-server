using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Contracts;

// Sink contract: push a batch to the target (DB, EventBus, API, etc.)
public interface ISystemLogSink
{
  Task WriteAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct);
}
