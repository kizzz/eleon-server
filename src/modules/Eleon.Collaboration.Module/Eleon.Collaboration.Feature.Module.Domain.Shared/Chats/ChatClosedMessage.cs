using System;

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class ChatClosedMessage
  {
    public ChatMemberInfo ClosedByUser { get; set; }
    public DateTime ChatCloseTime { get; set; }
  }
}
