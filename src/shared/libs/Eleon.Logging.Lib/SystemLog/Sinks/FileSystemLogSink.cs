using Eleon.Logging.Lib.SystemLog.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Sinks;

public sealed class FileSystemLogSink : ISystemLogSink, IDisposable
{
  private readonly string _path;
  private readonly bool _append;
  private readonly bool _flushPerBatch;
  private readonly object _lock = new();
  private StreamWriter? _writer;
  private bool _disposed;

  private static readonly JsonSerializerOptions _json = new()
  {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false
    // If you want enum as string, uncomment:
    // ,Converters = { new JsonStringEnumConverter() }
  };

  public FileSystemLogSink(string path, bool append = true, bool flushPerBatch = true)
  {
    if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
    _path = path;
    _append = append;
    _flushPerBatch = flushPerBatch;
  }

  public async Task WriteAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct)
  {
    if (_disposed || batch is null || batch.Count == 0) return;

    try
    {
      await WriteInternalAsync(batch, ct);
    }
    catch
    {
      // try once to reopen and retry
      ResetWriter();
      EnsureWriter();
      await WriteInternalAsync(batch, ct);
    }
  }

  private async Task WriteInternalAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct)
  {
    EnsureWriter();
    StreamWriter w;
    lock (_lock) w = _writer ?? throw new ObjectDisposedException(nameof(FileSystemLogSink));

    foreach (var e in batch)
    {
      ct.ThrowIfCancellationRequested();

      // NDJSON: one object per line
      var line = JsonSerializer.Serialize(new
      {
        e.Message,
        LogLevel = e.LogLevel.ToString(),
        Exception = e.Exception?.ToString(),
        e.Time,
        e.ApplicationName,
        e.ExtraProperties
      }, _json);

      await w.WriteLineAsync(line);
    }

    if (_flushPerBatch)
      await w.FlushAsync();
  }

  private void EnsureWriter()
  {
    lock (_lock)
    {
      if (_writer != null) return;

      var dir = Path.GetDirectoryName(_path);
      if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      // share for read so you can tail the log while app is running
      var fs = new FileStream(_path,
                              _append ? FileMode.Append : FileMode.Create,
                              FileAccess.Write,
                              FileShare.ReadWrite,
                              bufferSize: 64 * 1024,
                              useAsync: true);

      _writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
      {
        AutoFlush = false
      };
    }
  }

  private void ResetWriter()
  {
    lock (_lock)
    {
      try { _writer?.Dispose(); } catch { /* ignore */ }
      _writer = null;
    }
  }

  public void Dispose()
  {
    if (_disposed) return;
    _disposed = true;
    ResetWriter();
  }
}
