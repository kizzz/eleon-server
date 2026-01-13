using VPortal.Collaboration.Feature.Module.ChatRooms;

namespace VPortal.Collaboration.Feature.Module.DocumentConversations
{
  public class DocumentConversationInfoDto
  {
    public ChatRoomDto ChatRoom { get; set; }
    public bool IsMember { get; set; }
  }
}
