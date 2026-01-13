using System;

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class ChatCreatedMessage
  {
    public ChatMemberInfo CreatedByUser { get; set; }
    public DateTime ChatCreationTime { get; set; }
  }
}
