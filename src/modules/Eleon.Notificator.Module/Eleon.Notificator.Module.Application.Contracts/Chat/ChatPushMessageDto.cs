using Messaging.Module.ETO;

namespace VPortal.Notificator.Module.Chat
{
  public class ChatPushMessageDto
  {
    public ChatMessageEto Message { get; set; }
    public ChatRoomEto ChatRoom { get; set; }
    public bool IsChatMuted { get; set; }
  }
}
