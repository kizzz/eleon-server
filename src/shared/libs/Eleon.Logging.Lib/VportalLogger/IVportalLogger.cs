using Eleon.Logging.Lib.VportalLogger;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Logging.Module
{
  public interface IVportalLogger<T> : IVportalControllerLogger<T>
  {
    [Obsolete("Use ILogger<T> directly instead of accessing the raw logger.")]
    public ILogger Log { get; }

    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogStart/LogFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    void LogStart(object argsInfo = null, [CallerMemberName] string memberName = "");

    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogStart/LogFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    void LogFinish([CallerMemberName] string memberName = "");

    [DoesNotReturn]
    void Capture(Exception ex, [CallerMemberName] string memberName = "");

    void CaptureAndSuppress(Exception ex, [CallerMemberName] string memberName = "");
  }
}
