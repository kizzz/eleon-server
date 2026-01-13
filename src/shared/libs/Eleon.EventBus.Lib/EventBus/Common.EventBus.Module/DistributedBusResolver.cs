using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace Common.EventBus.Module
{
  public class DistributedBusResolver : ISingletonDependency
  {
    private readonly IOptions<EventBusOptions> options;
    private readonly IEnumerable<IDistributedBusResolveContributor> contributors;
    private readonly ICurrentTenant currentTenant;

    private List<ConnectedBus> connectedBuses = new List<ConnectedBus>();

    private SemaphoreSlim semaphore = new(1, 1);
    private bool initialized = false;

    public DistributedBusResolver(
        IEnumerable<IDistributedBusResolveContributor> contributors,
        ICurrentTenant currentTenant,
        IOptions<EventBusOptions> options)
    {
      this.contributors = contributors;
      this.options = options;
      this.currentTenant = currentTenant;
    }

    public async ValueTask ConnectEventBus(Guid? tenantId, EventBusOptions options, Guid? eventBusId = null)
    {
      eventBusId ??= Guid.Empty;

      var connected = connectedBuses.FirstOrDefault(x => eventBusId == x.BusId);
      if (connected != null)
      {
        return;
      }

      var contributor = contributors.FirstOrDefault(x => x.ProviderType == options.Provider);

      if (contributor == null)
      {
        throw new Exception("Unable to resolve an event bus in the current context.");
      }

      var bus = await contributor.Connect(options);

      connectedBuses.Add(new ConnectedBus(tenantId, eventBusId.Value, options, bus));
    }

    public async ValueTask<List<IDistributedEventBus>> ResolveTenantEventBuses()
    {
      await EnsureDefaultBusConnected();

      var buses = connectedBuses
          .Where(x => x.TenantId == null)//currentTenant.Id)
          .Select(x => x.Instance)
          .ToList();

      if (buses.IsNullOrEmpty())
      {
        throw new Exception("Unable to resolve an event bus in the current context.");
      }

      return buses;
    }

    public IDistributedEventBus? GetConnectedEventBus(Guid eventBusId)
    {
      return connectedBuses.FirstOrDefault(x => x.BusId == eventBusId)?.Instance;
    }

    public List<IDistributedEventBus> GetConnectedEventBuses()
    {
      return connectedBuses.Select(x => x.Instance).ToList();
    }

    internal async ValueTask EnsureDefaultBusConnected()
    {
      await semaphore.WaitAsync();

      try
      {
        if (!initialized)
        {
          await ConnectEventBus(null, options.Value);

          initialized = true;
        }
      }
      finally
      {
        semaphore.Release();
      }
    }

    private record ConnectedBus(Guid? TenantId, Guid BusId, EventBusOptions Options, IDistributedEventBus Instance);
  }
}
