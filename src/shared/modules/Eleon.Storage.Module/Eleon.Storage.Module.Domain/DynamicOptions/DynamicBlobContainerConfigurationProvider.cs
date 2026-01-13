using Microsoft.Extensions.Options;
using System;
using System.Threading;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace VPortal.Storage.Module.DynamicOptions;

public class DynamicBlobContainerConfigurationProvider : IBlobContainerConfigurationProvider, ITransientDependency
{
  protected IOptions<AbpBlobStoringOptions> Options { get; }
  private DateTime _lastRefreshTime = DateTime.MinValue;
  private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(5);
  private readonly object _refreshLock = new object();

  public DynamicBlobContainerConfigurationProvider(IOptions<AbpBlobStoringOptions> options)
  {
    Options = options;
  }

  public virtual BlobContainerConfiguration Get(string name)
  {
    // Only refresh if cache is stale to avoid blocking on every call
    var now = DateTime.UtcNow;
    if (now - _lastRefreshTime > _refreshInterval)
    {
      lock (_refreshLock)
      {
        // Double-check after acquiring lock
        if (now - _lastRefreshTime > _refreshInterval)
        {
          // Use ConfigureAwait(false) to avoid deadlocks and capture context issues
          Options.SetAsync().ConfigureAwait(false).GetAwaiter().GetResult();
          _lastRefreshTime = DateTime.UtcNow;
        }
      }
    }

    return Options.Value.Containers.GetConfiguration(name);
  }
}
