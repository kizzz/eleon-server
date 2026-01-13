using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services;

public enum ServiceStatus
{
  Unknown,
  Stopped,
  Starting,
  Running,       // alive (liveness) but readiness not guaranteed
  Ready,         // readiness probe passed
  Stopping,
  Failed
}

public class ManagedServiceOptions
{
  public required string Name { get; init; }
  public required string ExecutablePath { get; init; }
  public string? Arguments { get; init; }
  public string? WorkingDirectory { get; init; }
  public Dictionary<string, string>? Environment { get; init; }

  // Health (optional)
  public Uri? ReadinessHttpEndpoint { get; init; }   // e.g. https://localhost:5001/healthz
  public string? TcpHost { get; init; }              // optional TCP check
  public int? TcpPort { get; init; }

  // Lifecycle
  public TimeSpan StartTimeout { get; init; } = TimeSpan.FromSeconds(45);
  public TimeSpan StopTimeout { get; init; } = TimeSpan.FromSeconds(20);
  public TimeSpan ReadinessTimeout { get; init; } = TimeSpan.FromSeconds(30);
  public TimeSpan ReadinessPollInterval { get; init; } = TimeSpan.FromSeconds(1.0);

  // Optional graceful stop hooks
  public Uri? ShutdownHttpEndpoint { get; init; }    // call GET/POST to request shutdown
  public HttpMethod ShutdownHttpMethod { get; init; } = HttpMethod.Post;

  // Optional CLI stop
  public string? StopExecutablePath { get; init; }
  public string? StopArguments { get; init; }
}

internal sealed class TrackedProcess
{
  public required Process Process { get; init; }
  public required ManagedServiceOptions Options { get; init; }
  public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
  public volatile ServiceStatus Status = ServiceStatus.Starting;
  public CancellationTokenSource Cts { get; } = new();
}

public class ServiceLifecycleManager : ISingletonDependency
{
  private readonly ILogger<ServiceLifecycleManager> _log;
  private readonly ConcurrentDictionary<string, TrackedProcess> _services = new(StringComparer.OrdinalIgnoreCase);
  private readonly ConcurrentDictionary<string, object> _locks = new(StringComparer.OrdinalIgnoreCase);

  // Last known options per orchestrator service name (used by legacy StopAsync(name))
  private readonly ConcurrentDictionary<string, ManagedServiceOptions> _knownOptions =
      new(StringComparer.OrdinalIgnoreCase);

  private readonly HttpClient _http = new(new HttpClientHandler
  {
    // Accept self-signed for local dev; make configurable if needed
    ServerCertificateCustomValidationCallback = static (_, _, _, _) => true
  });

  public ServiceLifecycleManager(ILogger<ServiceLifecycleManager> log) => _log = log;

  private object GetLock(string name) => _locks.GetOrAdd(name, _ => new object());

  // ------------------ PUBLIC API ------------------

  public async Task<bool> StartAsync(ManagedServiceOptions options, bool waitForReady = true, CancellationToken cancellationToken = default)
  {
    var name = options.Name;

    // If tracked and alive → done
    lock (GetLock(name))
    {
      if (_services.TryGetValue(name, out var existing) && !existing.Process.HasExited)
      {
        _log.LogDebug("Service '{Name}' already running (PID {Pid}).", name, existing.Process.Id);
        _knownOptions[name] = options;
        return true;
      }
    }

    // Not tracked → try to ATTACH to an already-running OS process by executable
    if (TryAttachExistingProcess(options, out var attached))
    {
      _log.LogDebug("Attached to existing process for '{Name}' (PID {Pid}).", name, attached.Process.Id);
      lock (GetLock(name))
        _services[name] = attached;

      _knownOptions[name] = options;
      attached.Status = ServiceStatus.Running;
      return true;
    }

    _log.LogDebug("Starting service '{Name}'...", name);

    var psi = new ProcessStartInfo
    {
      FileName = options.ExecutablePath,
      Arguments = options.Arguments ?? string.Empty,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      RedirectStandardInput = false,
      CreateNoWindow = true,
      WorkingDirectory = options.WorkingDirectory ?? Environment.CurrentDirectory
    };

    if (options.Environment is not null)
    {
      foreach (var kv in options.Environment)
        psi.Environment[kv.Key] = kv.Value;
    }

    var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
    var tracked = new TrackedProcess { Process = process, Options = options, Status = ServiceStatus.Starting };

    // Pipe logs
    process.OutputDataReceived += (_, e) => { if (e.Data is not null) _log.LogDebug("[{Name}] {Line}", name, e.Data); };
    process.ErrorDataReceived += (_, e) => { if (e.Data is not null) _log.LogError("[{Name}] {Line}", name, e.Data); };

    if (!process.Start())
    {
      _log.LogError("Failed to start service '{Name}'.", name);
      return false;
    }
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    lock (GetLock(name))
      _services[name] = tracked;

    _knownOptions[name] = options;

    // Immediate liveness
    var startCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    startCts.CancelAfter(options.StartTimeout);

    await Task.Yield();
    if (process.HasExited)
    {
      _log.LogError("Service '{Name}' exited immediately with code {Code}.", name, process.ExitCode);
      tracked.Status = ServiceStatus.Failed;
      return false;
    }

    tracked.Status = ServiceStatus.Running;

    if (!waitForReady)
      return true;

    var ready = await WaitForReadinessAsync(tracked, startCts.Token);
    tracked.Status = ready ? ServiceStatus.Ready : ServiceStatus.Running;

    if (!ready)
      _log.LogWarning("Service '{Name}' did not become 'Ready' before timeout, but is running. Continuing.", name);

    return true;
  }

  /// <summary>
  /// NEW PRIMARY STOP: receives the same options as StartAsync so we can find the process
  /// by full path or file name with high confidence.
  /// </summary>
  public async Task<bool> StopAsync(ManagedServiceOptions options, bool force = false, CancellationToken cancellationToken = default)
  {
    var name = options.Name;

    // A) Tracked?
    TrackedProcess? tracked;
    lock (GetLock(name))
    {
      _services.TryGetValue(name, out tracked);
    }

    if (tracked is not null && !tracked.Process.HasExited)
    {
      return await StopTrackedAsync(name, tracked, force, cancellationToken);
    }

    // B) Not tracked → try to find running process by executable (full path/file name/process name)
    if (TryFindBestProcessMatch(options, out var proc))
    {
      try
      {
        _log.LogDebug("Stopping untracked '{Name}' by executable match (PID {Pid}).", name, proc.Id);

        // We can still try HTTP shutdown / CLI stop using the provided options
        await TryGracefulSignalsAsync(options, cancellationToken);

        var ok = await TryStopProcessAsync(proc, options.StopTimeout, cancellationToken, force);
        return ok;
      }
      finally { TryDispose(proc); }
    }

    _log.LogDebug("Untracked '{Name}': no running process matched executable '{Exe}'. Nothing to stop.", name, options.ExecutablePath);
    return true;
  }

  public bool EnsureTrackedIfRunning(ManagedServiceOptions options)
  {
    var name = options.Name;

    // Already tracked & alive?
    lock (GetLock(name))
    {
      if (_services.TryGetValue(name, out var existing) && !existing.Process.HasExited)
      {
        // refresh known options
        _knownOptions[name] = options;
        return true;
      }
    }

    // Try to attach to a running OS process that matches the executable (full path/file name/process name)
    if (TryFindBestProcessMatch(options, out var proc))
    {
      try
      {
        var tracked = new TrackedProcess
        {
          Process = proc,
          Options = options,
          Status = ServiceStatus.Running
        };

        lock (GetLock(name))
          _services[name] = tracked;

        _knownOptions[name] = options;
        return true;
      }
      catch
      {
        try { proc.Dispose(); } catch { /* ignore */ }
        return false;
      }
    }

    // Nothing to attach to
    return false;
  }

  public ServiceStatus GetStatus(string name)
  {
    lock (GetLock(name))
    {
      if (!_services.TryGetValue(name, out var tracked))
        return ServiceStatus.Stopped;

      if (tracked.Process.HasExited)
        return ServiceStatus.Stopped;

      return tracked.Status;
    }
  }

  public IEnumerable<(string Name, int Pid, ServiceStatus Status, DateTimeOffset StartedAt)> List()
  {
    foreach (var kv in _services)
    {
      var tp = kv.Value;
      var pid = tp.Process.HasExited ? -1 : tp.Process.Id;
      yield return (kv.Key, pid, GetStatus(kv.Key), tp.StartedAt);
    }
  }

  // ------------------ INTERNALS ------------------

  private async Task<bool> StopTrackedAsync(string name, TrackedProcess tracked, bool force, CancellationToken cancellationToken)
  {
    if (tracked.Process.HasExited)
    {
      _services.TryRemove(name, out _);
      return true;
    }

    _log.LogDebug("Stopping service '{Name}' (PID {Pid})...", name, tracked.Process.Id);
    tracked.Status = ServiceStatus.Stopping;

    var options = tracked.Options;

    // try app-level graceful signals (HTTP / CLI)
    await TryGracefulSignalsAsync(options, cancellationToken, force);

    // 3) CloseMainWindow (Windows / GUI-capable)
    if (!force && tracked.Process.CloseMainWindow())
    {
      if (await WaitForExitAsync(tracked.Process, options.StopTimeout, cancellationToken))
      {
        Cleanup(name, tracked);
        return true;
      }
    }

    // 4) Kill
    try
    {
      tracked.Process.Kill(entireProcessTree: true);
      if (await WaitForExitAsync(tracked.Process, TimeSpan.FromSeconds(5), cancellationToken))
      {
        Cleanup(name, tracked);
        return true;
      }
    }
    catch (Exception ex)
    {
      _log.LogError(ex, "Killing service '{Name}' failed.", name);
    }

    Cleanup(name, tracked);
    return tracked.Process.HasExited;
  }

  private async Task TryGracefulSignalsAsync(ManagedServiceOptions options, CancellationToken ct, bool force = false)
  {
    // 1) HTTP shutdown endpoint
    if (!force && options.ShutdownHttpEndpoint is not null)
    {
      try
      {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(options.StopTimeout);
        using var req = new HttpRequestMessage(options.ShutdownHttpMethod, options.ShutdownHttpEndpoint);
        var resp = await _http.SendAsync(req, cts.Token);
        _log.LogDebug("Shutdown HTTP returned {StatusCode}.", resp.StatusCode);
      }
      catch (Exception ex)
      {
        _log.LogWarning(ex, "Shutdown HTTP failed.");
      }
    }

    // 2) CLI stop command
    if (!force && options.StopExecutablePath is not null)
    {
      try
      {
        var ok = await RunShortProcessAsync(
            options.StopExecutablePath,
            options.StopArguments ?? string.Empty,
            options.WorkingDirectory,
            options.Environment,
            options.StopTimeout,
            ct);
        _log.LogDebug("Stop command {Result}.", ok ? "succeeded" : "failed");
      }
      catch (Exception ex)
      {
        _log.LogWarning(ex, "Stop command failed.");
      }
    }
  }

  private async Task<bool> WaitForReadinessAsync(TrackedProcess tracked, CancellationToken token)
  {
    var opt = tracked.Options;
    var started = Stopwatch.StartNew();
    while (!token.IsCancellationRequested && !tracked.Process.HasExited)
    {
      // HTTP readiness
      if (opt.ReadinessHttpEndpoint is not null)
      {
        try
        {
          using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
          cts.CancelAfter(TimeSpan.FromSeconds(3));
          using var resp = await _http.GetAsync(opt.ReadinessHttpEndpoint, cts.Token);
          if (resp.IsSuccessStatusCode)
            return true;
        }
        catch { /* retry */ }
      }

      // TCP readiness
      if (opt.TcpHost is not null && opt.TcpPort is not null)
      {
        try
        {
          using var tcp = new TcpClient();
          var connectTask = tcp.ConnectAsync(opt.TcpHost, opt.TcpPort.Value);
          var delayTask = Task.Delay(TimeSpan.FromSeconds(2), token);
          var done = await Task.WhenAny(connectTask, delayTask);
          if (done == connectTask && tcp.Connected)
            return true;
        }
        catch { /* retry */ }
      }

      if (started.Elapsed >= opt.ReadinessTimeout)
        return false;

      await Task.Delay(opt.ReadinessPollInterval, token);
    }
    return false;
  }

  private static async Task<bool> RunShortProcessAsync(
      string exe, string? args, string? cwd, Dictionary<string, string>? env,
      TimeSpan timeout, CancellationToken token)
  {
    var psi = new ProcessStartInfo
    {
      FileName = exe,
      Arguments = args ?? string.Empty,
      UseShellExecute = false,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      CreateNoWindow = true,
      WorkingDirectory = cwd ?? Environment.CurrentDirectory
    };
    if (env is not null)
      foreach (var kv in env)
        psi.Environment[kv.Key] = kv.Value;

    using var p = new Process { StartInfo = psi };
    if (!p.Start()) return false;

    var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    _ = Task.Run(async () =>
    {
      try
      {
        await Task.WhenAny(p.WaitForExitAsync(token), Task.Delay(timeout, token));
        tcs.TrySetResult(p.HasExited);
      }
      catch (Exception ex) { tcs.TrySetException(ex); }
    }, token);

    return await tcs.Task.ConfigureAwait(false);
  }

  private static async Task<bool> WaitForExitAsync(Process p, TimeSpan timeout, CancellationToken token)
  {
    try
    {
      using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
      cts.CancelAfter(timeout);
      await p.WaitForExitAsync(cts.Token);
      return true;
    }
    catch { return false; }
  }

  private void Cleanup(string name, TrackedProcess tracked)
  {
    tracked.Cts.Cancel();
    _services.TryRemove(name, out _);
    TryDispose(tracked.Process);
    _log.LogDebug("Service '{Name}' cleaned up.", name);
  }

  // ------------------ PROCESS MATCHING (robust, score-based) ------------------

  private bool TryAttachExistingProcess(ManagedServiceOptions options, out TrackedProcess tracked)
  {
    tracked = null!;
    if (!TryFindBestProcessMatch(options, out var p))
      return false;

    // We cannot rewire stdout/stderr for an already running process
    tracked = new TrackedProcess
    {
      Process = p,
      Options = options,
      Status = ServiceStatus.Running
    };
    return true;
  }

  /// <summary>
  /// Try to find a running process that best matches the given options.
  /// Preference: full path (exact) &gt; file name (exact) &gt; ends-with path &gt; process base name.
  /// </summary>
  private static bool TryFindBestProcessMatch(ManagedServiceOptions options, out Process process)
  {
    process = null!;
    var exePath = options.ExecutablePath;
    var exeFile = Path.GetFileName(exePath);
    var exeNoExt = Path.GetFileNameWithoutExtension(exePath);

    int bestScore = int.MinValue;
    Process? best = null;

    foreach (var p in Process.GetProcesses())
    {
      try
      {
        var score = ScoreProcessMatch(p, exePath, exeFile, exeNoExt);
        if (score > bestScore)
        {
          // Dispose previous loser
          TryDispose(best);
          bestScore = score;
          best = p;
        }
        else
        {
          TryDispose(p);
        }
      }
      catch
      {
        TryDispose(p);
      }
    }

    if (best is not null && bestScore > 0)
    {
      process = best;
      return true;
    }

    TryDispose(best);
    return false;
  }

  /// <summary>
  /// Higher is better. Returns 0 if no match.
  /// </summary>
  private static int ScoreProcessMatch(Process p, string exePath, string exeFile, string exeNoExt)
  {
    int score = 0;

    // 1) ProcessName (no extension) exact
    //if (!string.IsNullOrEmpty(p.ProcessName) &&
    //    p.ProcessName.Equals(exeNoExt, StringComparison.OrdinalIgnoreCase))
    //{
    //    score = Math.Max(score, 10);
    //}

    // 2) MainModule path/file checks (may throw on protected processes)
    string? mm = null;
    try { mm = p.MainModule?.FileName; } catch { /* access denied */ }

    if (!string.IsNullOrEmpty(mm))
    {
      // Full path exact
      if (mm.Equals(exePath, StringComparison.OrdinalIgnoreCase))
        score = Math.Max(score, 100);

      // File name exact
      //var mmFile = Path.GetFileName(mm);
      //if (mmFile.Equals(exeFile, StringComparison.OrdinalIgnoreCase))
      //    score = Math.Max(score, 80);

      // Ends-with path (handles differing roots / symlinks)
      //if (exePath.Length > 0 && mm.EndsWith(exePath, StringComparison.OrdinalIgnoreCase))
      //    score = Math.Max(score, 60);
    }

    return score;
  }

  private static Process? FindByProcessBaseName(string nameOrExe)
  {
    var baseName = Path.GetFileNameWithoutExtension(nameOrExe);
    foreach (var p in Process.GetProcesses())
    {
      try
      {
        if (p.ProcessName.Equals(baseName, StringComparison.OrdinalIgnoreCase))
          return p;
      }
      catch
      {
        TryDispose(p);
      }
    }
    return null;
  }

  private static async Task<bool> TryStopProcessAsync(Process p, TimeSpan gracefulTimeout, CancellationToken ct, bool force)
  {
    // 1) Gentle: CloseMainWindow (no-op for headless)
    if (!force && p.CloseMainWindow())
    {
      if (await WaitForExitAsync(p, gracefulTimeout, ct))
        return true;
    }

    // 2) Kill tree
    try
    {
      p.Kill(entireProcessTree: true);
      if (await WaitForExitAsync(p, TimeSpan.FromSeconds(5), ct))
        return true;
    }
    catch { /* ignore */ }

    return p.HasExited;
  }

  private static void TryDispose(Process? p)
  {
    try { p?.Dispose(); } catch { /* ignore */ }
  }
}
