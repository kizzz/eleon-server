using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Logger;
using System.Collections.Concurrent;

namespace Eleon.Logging.Lib.SystemLog.Sinks;

/// <summary>
/// Sends system logs to the distributed event bus, with bounded queue + cooldowns.
/// Ctor accepts only configuration; the event bus is retrieved via a static accessor.
/// </summary>
public abstract class AbstractQueuedSystemLogSink : ISystemLogSink
{
  // ---------- Options ----------
  private readonly int _queueCapacity;
  private readonly int _maxBatchSize;
  private readonly int _cooldownMinutes;

  // ---------- State ----------
  private readonly ConcurrentQueue<SystemLogEntry> _queue = new();
  private readonly object _gate = new(); // guards size trimming + state transitions
  private volatile bool _sinkAvailable = true;
  private DateTime _nextRetryAtUtc = DateTime.MinValue; // when we’re allowed to try again if unavailable
  private readonly SemaphoreSlim _flushLock = new(1, 1);

  public AbstractQueuedSystemLogSink(int queueCapacity = 5_000, int maxBatchSize = 512, int cooldownMinutes = 5)
  {
    _queueCapacity = Math.Max(1, queueCapacity);
    _maxBatchSize = Math.Max(1, maxBatchSize);
    _cooldownMinutes = Math.Clamp(cooldownMinutes, 1, 120);
  }

  // WriteAsync: enqueue, then best-effort flush without blocking the caller
  public Task WriteAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct)
  {
    if (ShouldFilterDisableNotification)
    {
      batch = batch
          .Where(e => e.ExtraProperties?.GetValueOrDefault(EleonsoftLog.DisableNotificationProperty) == null)
          .ToList();
    }

    if (batch is { Count: > 0 })
    {
      EnqueueAllWithTrim(batch);
    }

    if (!ShouldAttemptSendNow()) return Task.CompletedTask;

    _ = Task.Run(async () =>
    {
      try
      {
        await _flushLock.WaitAsync(ct).ConfigureAwait(false);
        try { await SendSnapshotAsync(ct).ConfigureAwait(false); }
        finally { _flushLock.Release(); }
      }
      catch { /* swallow – best effort */ }
    }, CancellationToken.None);

    return Task.CompletedTask;
  }

  // Manual flush: serialize with same lock, return per your contract
  public async Task<KeyValuePair<int, SystemLogEntry[]>> TryFlushNowAsync(CancellationToken ct = default)
  {
    if (_queue.IsEmpty) return new(0, Array.Empty<SystemLogEntry>());

    await _flushLock.WaitAsync(ct).ConfigureAwait(false);
    try
    {
      var (ok, sent) = await SendSnapshotAsync(ct).ConfigureAwait(false);
      return ok
          ? new(sent, Array.Empty<SystemLogEntry>())
          : new(sent, _queue.ToArray());
    }
    finally
    {
      _flushLock.Release();
    }
  }

  private bool ShouldAttemptSendNow()
  {
    if (_queue.IsEmpty) return false;
    if (_sinkAvailable) return true;
    return DateTime.UtcNow >= _nextRetryAtUtc;
  }

  // ---------------- Internals ----------------
  private void EnqueueAllWithTrim(IReadOnlyList<SystemLogEntry> batch)
  {
    lock (_gate)
    {
      foreach (var e in batch)
      {
        _queue.Enqueue(e);
      }

      // Trim oldest if over capacity
      while (_queue.Count > _queueCapacity && _queue.TryDequeue(out _)) { /* drop */ }
    }
  }


  /// <summary>
  /// Sends up to MaxBatchSize chunks until queue empty or a chunk fails.
  /// On success: dequeues sent items.
  /// On failure: leaves queue intact
  /// </summary>
  private async Task<(bool ok, int sentCount)> SendSnapshotAsync(CancellationToken ct)
  {
    var totalSent = 0;

    while (!_queue.IsEmpty)
    {
      ct.ThrowIfCancellationRequested();

      try
      {
        // Take a chunk without removing
        var chunk = PeekChunk(_maxBatchSize);
        if (chunk.Count == 0) break;

        // Map and send
        await SendChunckAsync(chunk, ct).ConfigureAwait(false);

        // Success → remove exactly chunk.Count items
        DequeueExactly(chunk.Count);
        totalSent += chunk.Count;
      }
      catch
      {
        // A failed attempt transitions to "unavailable" and sets next retry time.
        _sinkAvailable = false;
        // Two-stage: short quick retry delay the next time WriteAsync is called,
        // then minutes-long cooldown after that failure. Here we set the minutes cooldown directly.
        _nextRetryAtUtc = DateTime.UtcNow.AddMinutes(_cooldownMinutes);
        return (false, totalSent);
      }
    }

    _sinkAvailable = true;
    _nextRetryAtUtc = DateTime.MinValue;
    return (true, totalSent);
  }

  // Take up to n items without removing them
  private List<SystemLogEntry> PeekChunk(int n)
  {
    var list = new List<SystemLogEntry>(Math.Min(n, _queue.Count));
    foreach (var item in _queue)
    {
      list.Add(item);
      if (list.Count >= n) break;
    }
    return list;
  }

  // Remove exactly n items (assumes they’re present)
  private void DequeueExactly(int n)
  {
    lock (_gate)
    {
      for (var i = 0; i < n; i++)
      {
        _queue.TryDequeue(out _);
      }
    }
  }

  protected abstract Task SendChunckAsync(IReadOnlyList<SystemLogEntry> chunk, CancellationToken ct);
  protected abstract bool ShouldFilterDisableNotification { get; }
}
