using System.Collections.Generic;

namespace VPortal.Collaboration.Feature.Module.Chats
{
  public class MembersAddedMessage
  {
    public ChatMemberInfo AddedByUser { get; set; }

    public List<ChatMemberInfo> AddedUsers { get; set; }
  }
}
