using System.Threading.Channels;
using Eleon.Logging.Lib.SystemLog.Contracts;

namespace Eleon.Logging.Lib.SystemLog.Logger;

public static class EleonsoftLog
{
  private const int MAX_QUEUE_SIZE = 100_000;

  private static readonly object _gate = new();
  private static Channel<SystemLogEntry>? _channel;
  private static CancellationTokenSource? _cts;
  private static Thread? _worker;

  private static readonly List<ISystemLogSink> _sinks = new();
  private static readonly List<ISystemLogEnricher> _enrichers = new();

  public static EleonsoftLogOptions Options { get; private set; } = new();

  static EleonsoftLog()
  {
    if (_channel is not null) return;

    lock (_gate)
    {
      if (_channel is not null) return;

      Options = new EleonsoftLogOptions();

      _channel = Channel.CreateBounded<SystemLogEntry>(
          new BoundedChannelOptions(MAX_QUEUE_SIZE)
          {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
          });

      _cts ??= new CancellationTokenSource();

      // Ensure we still flush best-effort if the process exits before Configure
      AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeFlushOnExit();
    }
  }

  public static void Configure(EleonsoftLogOptions options)
  {
    lock (_gate)
    {
      // Update options even if worker is already started (for ForwardToSerilog flag)
      if (options != null)
      {
        Options = options;
      }
      else if (Options == null)
      {
        Options = new EleonsoftLogOptions();
      }

      // Only start worker once
      if (_worker is not null) return;

      // If Init() wasn't called, create the channel now
      if (_channel is null)
      {
        _channel = Channel.CreateBounded<SystemLogEntry>(
            new BoundedChannelOptions(MAX_QUEUE_SIZE)
            {
              SingleReader = true,
              SingleWriter = false,
              FullMode = BoundedChannelFullMode.DropOldest
            });
      }

      _cts ??= new CancellationTokenSource();

      _worker = new Thread(WorkerLoop) { IsBackground = true, Name = "EleonsoftLog-Worker" };
      _worker.Start();

      // Ensure graceful flush on exit as well
      AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeFlushOnExit();
    }
  }

  public static void AddSink(ISystemLogSink sink)
  {
    if (sink is null) throw new ArgumentNullException(nameof(sink));
    lock (_gate) _sinks.Add(sink);
  }

  public static void AddEnricher(ISystemLogEnricher enricher)
  {
    if (enricher is null) throw new ArgumentNullException(nameof(enricher));
    lock (_gate) _enrichers.Add(enricher);
  }

  // ---- Public logging API (all fields are strings except Level and TenantId per your spec) ----

  public static void Info(string message, Exception? exception = null) => Write(SystemLogLevel.Info, message, exception);

  public static void Warn(string message, Exception? exception = null) => Write(SystemLogLevel.Warning, message, exception);

  public static void Error(string message, Exception? ex = null) => Write(SystemLogLevel.Critical, message, ex);

  // ---- Core write ----

  public static void Write(
      SystemLogLevel level,
      string message,
      Exception? exception,
      Dictionary<string, string>? extraProps = null)
  {
    try
    {
      // Allow writes after Init() even if reader not started yet
      if (_channel is null) return;
      if (level < Options.LogLevel) return;

      var now = Options.NowProvider?.Invoke() ?? DateTimeOffset.UtcNow;

      // Merge extra properties (caller + global + correlation + enrichers)
      var props = new Dictionary<string, string>(StringComparer.Ordinal);

      if (extraProps is not null)
        foreach (var kv in extraProps)
          props[kv.Key] = kv.Value;

      // Call enrichers (mutate in-place)
      List<ISystemLogEnricher> enrSnapshot;
      lock (_gate) enrSnapshot = _enrichers.ToList();
      foreach (var enr in enrSnapshot)
      {
        try { enr.Enrich(props); } catch { /* ignore */ }
      }

      var entry = new SystemLogEntry(
          Message: message,
          LogLevel: level,
          Exception: exception,
          Time: now.DateTime,
          ApplicationName: Options.DefaultApplicationName,
          ExtraProperties: props
      );

      _channel.Writer.TryWrite(entry); // buffers until Configure starts the worker

      ForwardToSerilog(entry);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine("EleonsoftLog write error: " + ex);
    }
  }

  private static void ForwardToSerilog(SystemLogEntry entry)
  {
    // Skip forwarding to Serilog during early startup to prevent blocking
    // Serilog will be configured later in the startup process
    // This prevents hangs when EleonsoftLog is called before Serilog is initialized
    
    // Early return if forwarding is disabled - this is the primary guard
    if (Options == null || !Options.ForwardToSerilog)
      return;

    // Additional safety: wrap everything in try-catch to prevent any blocking
    // Even accessing Serilog.Log static properties might block if Serilog isn't ready
    try
    {
      var message = $"SYSLOG [{entry.LogLevel.ToString().ToUpper()} {entry.ApplicationName} {entry.Time}]: {entry.Message}\n{string.Join('\n', entry.ExtraProperties?.Select(x => $"{x.Key}: {x.Value}") ?? [])}\n{entry.Exception}";
      switch (entry.LogLevel)
      {
        case SystemLogLevel.Info:
          Serilog.Log.Information(message);
          break;
        case SystemLogLevel.Warning:
          Serilog.Log.Warning(message);
          break;
        case SystemLogLevel.Critical:
          Serilog.Log.Error(message);
          break;
      }
    }
    catch
    {
      // Silently ignore ALL exceptions - Serilog might not be initialized yet during early startup
      // This prevents blocking during configuration loading
      // Don't even log to Debug.WriteLine as that might also cause issues
    }
  }

  public static void FlushAndStop(TimeSpan? timeout = null)
  {
    if (_worker is null) return;

    lock (_gate)
    {
      try { _channel?.Writer.TryComplete(); } catch { /* ignore */ }
    }

    timeout ??= TimeSpan.FromSeconds(10);
    try { _cts?.CancelAfter(timeout.Value); } catch { /* ignore */ }
    try { _worker?.Join(timeout.Value); } catch { /* ignore */ }

    lock (_gate)
    {
      _cts?.Dispose();
      _cts = null;
      _worker = null;
      _channel = null;
      _sinks.Clear();
      _enrichers.Clear();
    }
  }

  private static void SafeFlushOnExit()
  {
    try { FlushAndStop(TimeSpan.FromSeconds(5)); } catch { /* best effort */ }
  }

  // ---- Background worker ----

  private static async void WorkerLoop()
  {
    var ct = _cts!.Token;
    var batch = new List<SystemLogEntry>(Options.BatchSize);
    var period = TimeSpan.FromMilliseconds(Options.BatchIntervalMs);
    var timer = new PeriodicTimer(period);

    try
    {
      while (await _channel.Reader.WaitToReadAsync(ct))
      {
        // Drain quickly
        while (_channel.Reader.TryRead(out var item))
        {
          batch.Add(item);
          if (batch.Count >= Options.BatchSize)
            await FlushAsync(batch, ct);
        }

        // Timed flush for leftovers
        if (batch.Count > 0 && await timer.WaitForNextTickAsync(ct))
          await FlushAsync(batch, ct);
      }
    }
    catch (OperationCanceledException) { /* shutdown */ }
    catch (Exception ex)
    {
      // last-resort: write to Debug to avoid recursion
      System.Diagnostics.Debug.WriteLine("EleonsoftLog worker error: " + ex);
    }
    finally
    {
      if (batch.Count > 0)
        await FlushAsync(batch, CancellationToken.None);
    }
  }

  private static async Task FlushAsync(List<SystemLogEntry> buf, CancellationToken ct)
  {
    if (buf.Count == 0) return;

    SystemLogEntry[] snapshot = buf.ToArray();
    buf.Clear();

    List<ISystemLogSink> sinksSnapshot;
    lock (_gate) sinksSnapshot = _sinks.ToList();

    List<Exception>? errors = null;
    foreach (var sink in sinksSnapshot)
    {
      try
      {
        await sink.WriteAsync(snapshot, ct);
      }
      catch (Exception ex)
      {
        (errors ??= new()).Add(new Exception($"{sink.GetType().Name} failed: {ex.Message}", ex));
      }
    }

    if (errors is not null)
      System.Diagnostics.Debug.WriteLine(string.Join(" | ", errors.Select(e => e.Message)));
  }

  public const string DisableNotificationProperty = "DisableNotification";

  public static ISystemLogSink[] GetSinksSnapshot() { lock (_gate) return _sinks.ToArray(); }
}
