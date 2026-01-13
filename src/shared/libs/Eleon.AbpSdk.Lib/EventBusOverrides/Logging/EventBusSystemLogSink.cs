using Common.EventBus.Module;
using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Extensions;
using Eleon.Logging.Lib.SystemLog.Sinks;
using EleonsoftModuleCollector.Commons.Module.Messages.SystemLog;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.SystemLog;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace SharedModule.modules.Logging.Module.SystemLog.Sinks;

/// <summary>
/// Sends system logs to the distributed event bus, with bounded queue + cooldowns.
/// Ctor accepts only configuration; the event bus is retrieved via a static accessor.
/// </summary>
public sealed class EventBusSystemLogSink : AbstractQueuedSystemLogSink
{
  private IDistributedEventBus _cachedBus;
  private IServiceScope _cachedScope;

  protected override bool ShouldFilterDisableNotification => true;

  public EventBusSystemLogSink(int queueCapacity = 5_000, int maxBatchSize = 64, int cooldownMinutes = 5) : base(queueCapacity, maxBatchSize, cooldownMinutes)
  {
  }

  protected override async Task SendChunckAsync(IReadOnlyList<SystemLogEntry> chunk, CancellationToken ct)
  {
    try
    {
      _cachedScope ??= StaticServicesAccessor.CreateScope();
      _cachedBus ??= _cachedScope.ServiceProvider.GetRequiredService<IDistributedEventBus>();

      // Map and send
      var logs = chunk.Select(MapToEto).ToList();
      var msg = new AddSystemLogMsg { Logs = logs };

      // Use extension method from Common.EventBus.Module if available
      if (_cachedBus is IResponseCapableEventBus responseBus)
      {
        var response = await responseBus.RequestAsync<AddSystemLogResponseMsg>(msg, 60).ConfigureAwait(false);
        if (response?.Success != true)
        {
          throw new Exception("Failed to send log notifications via event bus.");
        }
      }
      else
      {
        // Fallback: just publish without waiting for response
        await _cachedBus.PublishAsync(msg).ConfigureAwait(false);
      }
    }
    catch (Exception)
    {
      _cachedBus = null; // force recreate on next attempt
      _cachedScope = null;
      throw;
    }
  }

  /// <summary>
  /// Central transformation SystemLogEntry -> EleonsoftNotification.
  /// </summary>
  private AddSystemLogEto MapToEto(SystemLogEntry entry)
  {
    var extras = new Dictionary<string, string>(entry.ExtraProperties ?? new Dictionary<string, string>())
    {
      ["ApplicationName"] = entry.ApplicationName ?? string.Empty,
      ["Time"] = entry.Time.ToString("o") ?? DateTime.UtcNow.ToString("o")
    };

    extras.AddException(entry.Exception);

    return new AddSystemLogEto
    {
      ApplicationName = entry.ApplicationName ?? string.Empty,
      LogLevel = entry.LogLevel,
      Message = entry.Message,
      ExtraProperties = extras.ToDictionary(kv => kv.Key, kv => (object)kv.Value)
    };
  }
}
