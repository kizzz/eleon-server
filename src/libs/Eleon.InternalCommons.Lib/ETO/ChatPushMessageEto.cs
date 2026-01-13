namespace Messaging.Module.ETO
{
  public class ChatPushMessageEto
  {
    public ChatMessageEto Message { get; set; }
    public ChatRoomEto ChatRoom { get; set; }
    public List<Guid> MutedUsers { get; set; }
  }
}
