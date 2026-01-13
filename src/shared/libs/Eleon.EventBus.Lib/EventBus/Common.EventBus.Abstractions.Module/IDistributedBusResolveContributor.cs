using Common.Module.Constants;
using Volo.Abp.EventBus.Distributed;

namespace Common.EventBus.Module
{
  public interface IDistributedBusResolveContributor
  {
    EventBusProvider ProviderType { get; }
    Task<IDistributedEventBus> Connect(EventBusOptions options);
  }
}
