using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class DefaultTenantLanguageUpdatedMsg : VportalEvent
  {
    public string CultureName { get; set; }
    public string UiCultureName { get; set; }
  }
}
