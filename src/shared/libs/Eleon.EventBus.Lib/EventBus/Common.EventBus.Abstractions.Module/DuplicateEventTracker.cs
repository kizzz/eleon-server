using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace Common.EventBus.Module
{
  public class DuplicateEventTracker : ISingletonDependency
  {
    private const int CacheClearIntervalMinutes = 5;
    private readonly ConcurrentDictionary<Guid, DateTime> registeredMessages = new();

    private object clearTimeLock = new object();
    private DateTime lastClearTime = DateTime.UtcNow;

    public bool RegisterMessage(Guid eventId, DateTime eventSendTime)
    {
      DateTime now = DateTime.UtcNow;

      bool eventOutdated = now.Subtract(eventSendTime) > TimeSpan.FromMinutes(CacheClearIntervalMinutes);
      if (eventOutdated)
      {
        return false;
      }

      bool registered = registeredMessages.AddIfNotContains(KeyValuePair.Create(eventId, eventSendTime));

      bool doClear = false;
      lock (clearTimeLock)
      {
        now = DateTime.UtcNow;

        bool isTimeToClear = now.Subtract(lastClearTime) > TimeSpan.FromMinutes(CacheClearIntervalMinutes);
        if (isTimeToClear)
        {
          lastClearTime = now;
          doClear = true;
        }
      }

      if (doClear)
      {
        ClearOutdated(now);
      }

      return registered;
    }

    private void ClearOutdated(DateTime now)
    {
      var outdated = registeredMessages.Where(x => now.Subtract(x.Value) > TimeSpan.FromMinutes(CacheClearIntervalMinutes));
      foreach (var message in outdated)
      {
        registeredMessages.Remove(message.Key, out _);
      }
    }
  }
}
