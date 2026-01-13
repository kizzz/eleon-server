using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ModuleCollector.EventManagementModule.EventManagementModule.Module.EntityFrameworkCore.Repositories;

internal class SemaphoreInfo
{
  public SemaphoreSlim Semaphore { get; }
  public DateTime LastUsage { get; set; }

  public SemaphoreInfo(SemaphoreSlim semaphore, DateTime lastUsage)
  {
    Semaphore = semaphore;
    LastUsage = lastUsage;
  }
}

public class ConcurrencyManager : ISingletonDependency
{
  private static readonly ConcurrentDictionary<string, SemaphoreInfo> _locks
      = new ConcurrentDictionary<string, SemaphoreInfo>();

  public ConcurrencyManager() { }

  public SemaphoreSlim GetLock(string key)
  {
    var info = _locks.GetOrAdd(key, _ => new SemaphoreInfo(new SemaphoreSlim(1, 1), DateTime.UtcNow));
    info.LastUsage = DateTime.UtcNow;
    return info.Semaphore;
  }

  public Task<SemaphoreSlim> GetLockAsync(string key)
  {
    var info = _locks.GetOrAdd(key, _ => new SemaphoreInfo(new SemaphoreSlim(1, 1), DateTime.UtcNow));
    info.LastUsage = DateTime.UtcNow;
    return Task.FromResult(info.Semaphore);
  }

  public void ClearOldSemaphores(TimeSpan maxAge)
  {
    var now = DateTime.UtcNow;

    foreach (var kv in _locks.ToArray())
    {
      var key = kv.Key;
      var info = kv.Value;

      if (now - info.LastUsage > maxAge && info.Semaphore.CurrentCount == 1)
      {
        if (_locks.TryRemove(key, out var removed))
        {
          removed.Semaphore.Dispose();
        }
      }
    }
  }
}
