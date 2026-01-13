

namespace Messaging.Module.Messages;
[Common.Module.Events.DistributedEvent]
public class UseDedicatedGotMsg : VportalEvent
{
  public bool Success { get; set; }
}

[Common.Module.Events.DistributedEvent]
public class UseDedicatedMsg : VportalEvent
{
  public string ApplicationName { get; set; }
  public bool UseDedicated { get; set; }
}
