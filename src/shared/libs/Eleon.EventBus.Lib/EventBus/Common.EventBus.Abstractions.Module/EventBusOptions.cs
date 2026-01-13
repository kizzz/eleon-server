using Common.Module.Constants;

namespace Common.EventBus.Module
{
  public class EventBusOptions
  {
    public EventBusProvider Provider { get; set; }
    public required string ProviderOptionsJson { get; set; }
  }
}
