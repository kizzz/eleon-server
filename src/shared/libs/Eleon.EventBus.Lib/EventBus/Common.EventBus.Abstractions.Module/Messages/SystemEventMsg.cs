using Common.Module.Events;

namespace Messaging.Module.Messages
{
  [DistributedEvent]
  public class SystemEventMsg : VportalEvent
  {
    public string MessageData { get; set; }
    public string MessageType { get; set; }
    public SystemEventMsg() { }

    public SystemEventMsg(string name, string data)
    {
      MessageType = name;
      MessageData = data;
    }
  }
}
