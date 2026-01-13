using System.Collections.Generic;

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class MembersKickedMessage
  {
    public ChatMemberInfo KickedByUser { get; set; }

    public List<ChatMemberInfo> KickedUsers { get; set; }
  }
}
