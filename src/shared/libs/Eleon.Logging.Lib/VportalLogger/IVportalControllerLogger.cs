using System.Runtime.CompilerServices;

namespace Eleon.Logging.Lib.VportalLogger
{
  public interface IVportalControllerLogger<T>
  {
    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogHttpMethodStart/LogHttpMethodFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    void LogHttpMethodStart([CallerMemberName] string memberName = "");
    /// <summary>
    /// No-op; kept for temporary compatibility. Do not use.
    /// </summary>
    [Obsolete("VportalLogger.LogHttpMethodStart/LogHttpMethodFinish are deprecated. Use boundary logging (HTTP middleware / SignalR filter / job & consumer boundary scopes).", false)]
    void LogHttpMethodFinish([CallerMemberName] string memberName = "");
  }
}
