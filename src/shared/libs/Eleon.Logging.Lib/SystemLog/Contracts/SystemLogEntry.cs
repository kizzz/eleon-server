using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Contracts;

public sealed record SystemLogEntry(
    string Message,
    SystemLogLevel LogLevel,
    Exception? Exception,
    DateTime Time,
    string? ApplicationName,
    Dictionary<string, string>? ExtraProperties
);
