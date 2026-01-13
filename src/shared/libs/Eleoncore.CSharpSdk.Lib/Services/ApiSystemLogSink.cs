using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Extensions;
using Eleon.Logging.Lib.SystemLog.Sinks;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedModule.modules.Logging.Module.SystemLog.Sinks;

/// <summary>
/// Sends system logs to the distributed event bus, with bounded queue + cooldowns.
/// Ctor accepts only configuration; the event bus is retrieved via a static accessor.
/// </summary>
public sealed class ApiSystemLogSink : AbstractQueuedSystemLogSink
{
  private SystemLogApi _cachedApi;
  private readonly Func<SystemLogApi> _apiProvider;

  protected override bool ShouldFilterDisableNotification => true;

  public ApiSystemLogSink(
      Func<SystemLogApi>? apiProvider = null,
      int queueCapacity = 5_000, int maxBatchSize = 128, int cooldownMinutes = 5) : base(queueCapacity, maxBatchSize, cooldownMinutes)
  {
    _apiProvider = apiProvider ?? (() => new SystemLogApi());
  }

  protected override async Task SendChunckAsync(IReadOnlyList<SystemLogEntry> chunk, CancellationToken ct)
  {
    _cachedApi ??= _apiProvider();

    var dtos = chunk.Select(MapToDto).ToList();
    _cachedApi.UseApiAuth();
    var resp = await _cachedApi.DocMessageLogSystemLogWriteManyAsync(dtos, ct).ConfigureAwait(false);

    if (!resp.IsSuccessStatusCode)
    {
      _cachedApi = null; // force recreate on next attempt
      throw new Exception($"Failed to send system log batch: {(int)resp.StatusCode} {resp.ReasonPhrase}");
    }
  }

  /// <summary>
  /// Central place to transform SystemLogEntry into NotificatorNotificationDto.
  /// </summary>
  private EleonsoftModuleCollectorCreateSystemLogDto MapToDto(SystemLogEntry entry)
  {
    var props = new Dictionary<string, string>(entry.ExtraProperties ?? new Dictionary<string, string>())
        {
            { "Time", entry.Time.ToString("o") ?? DateTime.UtcNow.ToString("o") },
        };

    props.AddException(entry.Exception);

    var dto = new EleonsoftModuleCollectorCreateSystemLogDto
    {
      ApplicationName = entry.ApplicationName,
      LogLevel = (EleonSystemLogLevel)entry.LogLevel,
      Message = entry.Message,
      ExtraProperties = props
    };
    return dto;
  }
}
